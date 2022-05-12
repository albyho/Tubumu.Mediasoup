using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using Tubumu.Utils.Json;

namespace Tubumu.Mediasoup
{
    public abstract class ChannelBase : IChannel
    {
        #region Constants

        protected const int MessageMaxLen = PayloadMaxLen + sizeof(int);

        protected const int PayloadMaxLen = 1024 * 1024 * 4;

        #endregion Constants

        #region Protected Fields

        /// <summary>
        /// Logger
        /// </summary>
        protected readonly ILogger<Channel> _logger;

        /// <summary>
        /// Closed flag.
        /// </summary>
        protected bool _closed;

        /// <summary>
        /// Close locker.
        /// </summary>
        protected readonly AsyncReaderWriterLock _closeLock = new();

        /// <summary>
        /// Worker id.
        /// </summary>
        protected readonly int _workerId;

        /// <summary>
        /// Next id for messages sent to the worker process.
        /// </summary>
        protected uint _nextId = 0;

        /// <summary>
        /// Map of pending sent requests.
        /// </summary>
        protected readonly ConcurrentDictionary<uint, Sent> _sents = new();

        #endregion Protected Fields

        #region Events

        public event Action<string, string, string?>? MessageEvent;

        #endregion Events

        public ChannelBase(ILogger<Channel> logger, int workerId)
        {
            _logger = logger;
            _workerId = workerId;
        }

        public async Task CloseAsync()
        {
            _logger.LogDebug($"CloseAsync() | Worker[{_workerId}]");

            using (await _closeLock.WriteLockAsync())
            {
                if (_closed)
                {
                    return;
                }

                _closed = true;

                Cleanup();
            }
        }

        public virtual void Cleanup()
        {

        }

        private RequestMessage CreateRequestMessage(MethodId methodId, object? @internal = null, object? data = null)
        {
            var id = InterlockedExtensions.Increment(ref _nextId);
            var method = methodId.GetEnumMemberValue();

            var requestMesssge = new RequestMessage
            {
                Id = id,
                Method = method,
                Internal = @internal,
                Data = data,
            };

            return requestMesssge;
        }

        protected abstract void SendRequestMessage(RequestMessage requestMessage, Sent sent);

        public async Task<string?> RequestAsync(MethodId methodId, object? @internal = null, object? data = null)
        {
            _logger.LogDebug($"RequestAsync() | Worker[{_workerId}] Method:{methodId.GetEnumMemberValue()}");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Channel closed");
                }

                var requestMessage = CreateRequestMessage(methodId, @internal, data);

                var tcs = new TaskCompletionSource<string?>();
                var sent = new Sent
                {
                    RequestMessage = requestMessage,
                    Resolve = data =>
                    {
                        if (!_sents.TryRemove(requestMessage.Id!.Value, out _))
                        {
                            tcs.TrySetException(new Exception($"Received response does not match any sent request [id:{requestMessage.Id}]"));
                            return;
                        }
                        tcs.TrySetResult(data);
                    },
                    Reject = e =>
                    {
                        if (!_sents.TryRemove(requestMessage.Id!.Value, out _))
                        {
                            tcs.TrySetException(new Exception($"Received response does not match any sent request [id:{requestMessage.Id}]"));
                            return;
                        }
                        tcs.TrySetException(e);
                    },
                    Close = () =>
                    {
                        tcs.TrySetException(new InvalidStateException("Channel closed"));
                    },
                };
                if (!_sents.TryAdd(requestMessage.Id!.Value, sent))
                {
                    throw new Exception($"Error add sent request [id:{requestMessage.Id}]");
                }
                tcs.WithTimeout(TimeSpan.FromSeconds(15 + (0.1 * _sents.Count)), () => _sents.TryRemove(requestMessage.Id!.Value, out _));

                SendRequestMessage(requestMessage, sent);

                return await tcs.Task;
            }
        }

        #region Event handles

        public void ProcessMessage(string message)
        {
            try
            {
                // We can receive JSON messages (Channel messages) or log strings.
                var log = $"ProcessMessage() | Worker[{_workerId}] payload: {message}";
                switch (message[0])
                {
                    // 123 = '{' (a Channel JSON messsage).
                    case '{':
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            ProcessJson(message);
                        });
                        break;

                    // 68 = 'D' (a debug log).
                    case 'D':
                        if (!message.Contains("(trace)"))
                        {
                            _logger.LogDebug(log);
                        }

                        break;

                    // 87 = 'W' (a warn log).
                    case 'W':
                        if (!message.Contains("no suitable Producer"))
                        {
                            _logger.LogWarning(log);
                        }

                        break;

                    // 69 = 'E' (an error log).
                    case 'E':
                        _logger.LogError(log);
                        break;

                    // 88 = 'X' (a dump log).
                    case 'X':
                        _logger.LogDebug(log);
                        break;

                    default:
                        _logger.LogWarning($"ProcessMessage() | Worker[{_workerId}] unexpected data, message: {message}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProcessMessage() | Worker[{_workerId}] Received invalid message from the worker process, message: {message}");
                return;
            }
        }

        private void ProcessJson(string payload)
        {
            var jsonDocument = JsonDocument.Parse(payload);
            var msg = jsonDocument.RootElement;
            var id = msg.GetNullableJsonElement("id")?.GetNullableUInt32();
            var accepted = msg.GetNullableJsonElement("accepted")?.GetNullableBool();
            // targetId 可能是 Number 或 String。不能使用 GetString()，否则可能报错：Cannot get the value of a token type 'Number' as a string"
            var targetId = msg.GetNullableJsonElement("targetId")?.ToString();
            var @event = msg.GetNullableJsonElement("event")?.GetString();
            var error = msg.GetNullableJsonElement("error")?.GetString();
            var reason = msg.GetNullableJsonElement("reason")?.GetString();
            var data = msg.GetNullableJsonElement("data")?.ToString();

            // If a response, retrieve its associated request.
            if (id.HasValue && id.Value >= 0)
            {
                if (!_sents.TryGetValue(id.Value, out var sent))
                {
                    _logger.LogError($"ProcessMessage() | Worker[{_workerId}] Received response does not match any sent request [id:{id}], payload:{payload}");
                    return;
                }

                if (accepted.HasValue && accepted.Value)
                {
                    _logger.LogDebug($"ProcessMessage() | Worker[{_workerId}] Request succeed [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]");
                    sent.Resolve?.Invoke(data);
                }
                else if (!error.IsNullOrWhiteSpace())
                {
                    // 在 Node.js 实现中，error 的值可能是 "Error" 或 "TypeError"。
                    _logger.LogWarning($"ProcessMessage() | Worker[{_workerId}] Request failed [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]: {reason}. payload:{payload}");

                    sent.Reject?.Invoke(new Exception($"Request failed [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]: {reason}. payload:{payload}"));
                }
                else
                {
                    _logger.LogError($"ProcessMessage() | Worker[{_workerId}] Received response is not accepted nor rejected [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]. payload:{payload}");

                    sent.Reject?.Invoke(new Exception($"Received response is not accepted nor rejected [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]. payload:{payload}"));
                }
            }
            // If a notification emit it to the corresponding entity.
            else if (!targetId.IsNullOrWhiteSpace() && !@event.IsNullOrWhiteSpace())
            {
                MessageEvent?.Invoke(targetId!, @event!, data);
            }
            // Otherwise unexpected message.
            else
            {
                _logger.LogError($"ProcessMessage() | Worker[{_workerId}] Received message is not a response nor a notification: {payload}");
            }
        }

        #endregion Event handles
    }
}
