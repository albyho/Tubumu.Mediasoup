using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tubumu.Libuv;
using Tubumu.Utils.Extensions;
using Tubumu.Utils.Extensions.Object;
using Tubumu.Utils.Json;

namespace Tubumu.Mediasoup
{
    public class Channel
    {
        #region Constants

        private const int MessageMaxLen = 4194308;

        private const int PayloadMaxLen = 4194304;

        private const int RecvBufferMaxLen = 4194308 * 2;

        #endregion Constants

        #region Private Fields

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<Channel> _logger;

        // TODO: (alby) _closed 的使用及线程安全。
        /// <summary>
        /// Closed flag.
        /// </summary>
        private bool _closed;

        /// <summary>
        /// Unix Socket instance for sending messages to the worker process.
        /// </summary>
        private readonly UVStream _producerSocket;

        /// <summary>
        /// Unix Socket instance for receiving messages to the worker process.
        /// </summary>
        private readonly UVStream _consumerSocket;

        /// <summary>
        /// Worker process PID.
        /// </summary>
        private readonly int _processId;

        /// <summary>
        /// Next id for messages sent to the worker process.
        /// </summary>
        private uint _nextId = 0;

        /// <summary>
        /// Map of pending sent requests.
        /// </summary>
        private readonly ConcurrentDictionary<uint, Sent> _sents = new();

        /// <summary>
        /// Buffer for reading messages from the worker.
        /// </summary>
        private readonly byte[] _recvBuffer;
        private int _recvBufferCount;

        #endregion Private Fields

        #region Events

        public event Action<string, string, string?>? MessageEvent;

        #endregion Events

        public Channel(ILogger<Channel> logger, UVStream producerSocket, UVStream consumerSocket, int processId)
        {
            _logger = logger;

            _producerSocket = producerSocket;
            _consumerSocket = consumerSocket;
            _processId = processId;

            _recvBuffer = new byte[RecvBufferMaxLen];
            _recvBufferCount = 0;

            _consumerSocket.Data += ConsumerSocketOnData;
            _consumerSocket.Closed += ConsumerSocketOnClosed;
            _consumerSocket.Error += ConsumerSocketOnError;
            _producerSocket.Closed += ProducerSocketOnClosed;
            _producerSocket.Error += ProducerSocketOnError;
        }

        public void Close()
        {
            if (_closed)
            {
                return;
            }

            _logger.LogDebug($"Close() | Worker [pid:{_processId}]");

            _closed = true;

            // Close every pending sent.
            try
            {
                _sents.Values.ForEach(m => m.Close.Invoke());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Close() | Worker [pid:{_processId}] _sents.Values.ForEach(m => m.Close.Invoke())");
            }

            // Remove event listeners but leave a fake 'error' hander to avoid
            // propagation.
            _consumerSocket.Closed -= ConsumerSocketOnClosed;
            _consumerSocket.Error -= ConsumerSocketOnError;

            _producerSocket.Closed -= ProducerSocketOnClosed;
            _producerSocket.Error -= ProducerSocketOnError;

            // Destroy the socket after a while to allow pending incoming messages.
            // 在 Node.js 实现中，延迟了 200 ms。
            try
            {
                _producerSocket.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Close() | Worker [pid:{_processId}] _producerSocket.Close()");
            }

            try
            {
                _consumerSocket.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Close() | Worker [pid:{_processId}] _consumerSocket.Close()");
            }
        }

        public Task<string?> RequestAsync(MethodId methodId, object? @internal = null, object? data = null)
        {
            if (_closed)
            {
                throw new InvalidStateException("Channel closed");
            }

            var method = methodId.GetEnumMemberValue();
            var id = InterlockedExtensions.Increment(ref _nextId);
            // NOTE: For testinng
            //_logger.LogDebug($"RequestAsync() | [Method:{method}, Id:{id}]");

            var requestMesssge = new RequestMessage
            {
                Id = id,
                Method = method,
                Internal = @internal,
                Data = data,
            };

            var payload = requestMesssge.ToJson();
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            if (payloadBytes.Length > PayloadMaxLen)
            {
                throw new Exception("Channel request too big");
            }
            var payloadBytesLengthBytes = BitConverter.GetBytes(payloadBytes.Length);
            if (payloadBytes.Length + payloadBytesLengthBytes.Length > MessageMaxLen)
            {
                throw new Exception("Channel request too big");
            }

            var message = new byte[payloadBytes.Length + payloadBytesLengthBytes.Length];
            Array.Copy(payloadBytesLengthBytes, 0, message, 0, payloadBytesLengthBytes.Length);
            Array.Copy(payloadBytes, 0, message, payloadBytesLengthBytes.Length, payloadBytes.Length);

            var tcs = new TaskCompletionSource<string?>();

            var sent = new Sent
            {
                RequestMessage = requestMesssge,
                Resolve = data =>
                {
                    if (!_sents.TryRemove(id, out _))
                    {
                        tcs.TrySetException(new Exception($"Received response does not match any sent request [id:{id}]"));
                        return;
                    }
                    tcs.TrySetResult(data);
                },
                Reject = e =>
                {
                    if (!_sents.TryRemove(id, out _))
                    {
                        tcs.TrySetException(new Exception($"Received response does not match any sent request [id:{id}]"));
                        return;
                    }
                    tcs.TrySetException(e);
                },
                Close = () =>
                {
                    tcs.TrySetException(new InvalidStateException("Channel closed"));
                },
            };
            if (!_sents.TryAdd(id, sent))
            {
                throw new Exception($"Error add sent request [id:{id}]");
            }

            tcs.WithTimeout(TimeSpan.FromSeconds(15 + (0.1 * _sents.Count)), () => _sents.TryRemove(id, out _));

            Loop.Default.Sync(() =>
            {
                try
                {
                    // This may throw if closed or remote side ended.
                    _producerSocket.Write(message, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, $"_producerSocket.Write() | Worker [pid:{_processId}] Error");
                            sent.Reject(ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"_producerSocket.Write() | Worker [pid:{_processId}] Error");
                    sent.Reject(ex);
                }
            });

            return tcs.Task;
        }

        #region Event handles

        private void ConsumerSocketOnData(ArraySegment<byte> data)
        {
            if (data.Count > MessageMaxLen)
            {
                _logger.LogError($"ConsumerSocketOnData() | Worker [pid:{_processId}] Receiving data too large, ignore it");
                return;
            }

            // 数据回调通过单一线程进入，所有 _recvBuffer 是线程安全的。
            if (_recvBufferCount + data.Count > RecvBufferMaxLen)
            {
                _logger.LogError($"ConsumerSocketOnData() | Worker [pid:{_processId}] Receiving buffer is full, discarding all data into it");
                return;
            }

            Array.Copy(data.Array!, data.Offset, _recvBuffer, _recvBufferCount, data.Count);
            _recvBufferCount += data.Count;

            try
            {
                var readCount = 0;
                while (readCount < _recvBufferCount - sizeof(int) - 1)
                {
                    var msgLen = BitConverter.ToInt32(_recvBuffer, readCount);
                    readCount += sizeof(int);
                    if (readCount >= _recvBufferCount)
                    {
                        // Incomplete data.
                        break;
                    }

                    var payload = new byte[msgLen];
                    Array.Copy(_recvBuffer, readCount, payload, 0, msgLen);
                    readCount += msgLen;

                    var payloadString = Encoding.UTF8.GetString(payload, 0, payload.Length);

                    try
                    {
                        // We can receive JSON messages (Channel messages) or log strings.
                        var message = $"ConsumerSocketOnData() | Worker [pid:{_processId}] payload: {payloadString}";
                        switch (payloadString[0])
                        {
                            // 123 = '{' (a Channel JSON messsage).
                            case '{':
                                ThreadPool.QueueUserWorkItem(_ =>
                                {
                                    ProcessMessage(payloadString);
                                });
                                break;

                            // 68 = 'D' (a debug log).
                            case 'D':
                                if (!payloadString.Contains("(trace)"))
                                {
                                    _logger.LogDebug(message);
                                }

                                break;

                            // 87 = 'W' (a warn log).
                            case 'W':
                                if (!payloadString.Contains("no suitable Producer"))
                                {
                                    _logger.LogWarning(message);
                                }

                                break;

                            // 69 = 'E' (an error log).
                            case 'E':
                                _logger.LogError(message);
                                break;

                            // 88 = 'X' (a dump log).
                            case 'X':
                                _logger.LogDebug(message);
                                break;

                            default:
                                _logger.LogWarning($"ConsumerSocketOnData() | Worker [pid:{_processId}] unexpected data, payload: {payloadString}");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"ConsumerSocketOnData() | Worker [pid:{_processId}] Received invalid message from the worker process, payload: {payloadString}");
                        return;
                    }
                }

                var remainingLength = _recvBufferCount - readCount;
                if (remainingLength == 0)
                {
                    _recvBufferCount = 0;
                }
                else
                {
                    var temp = new byte[remainingLength];
                    Array.Copy(_recvBuffer, readCount, temp, 0, remainingLength);
                    Array.Copy(temp, 0, _recvBuffer, 0, remainingLength);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ConsumerSocketOnData() | Worker [pid:{_processId}] Invalid data received from the worker process.");
                return;
            }
        }

        private void ConsumerSocketOnClosed()
        {
            _logger.LogDebug($"ConsumerSocketOnClosed() | Worker [pid:{_processId}] Consumer Channel ended by the worker process");
        }

        private void ConsumerSocketOnError(Exception? exception)
        {
            _logger.LogDebug(exception, $"ConsumerSocketOnError() | Worker [pid:{_processId}] Consumer Channel error");
        }

        private void ProducerSocketOnClosed()
        {
            _logger.LogDebug($"ProducerSocketOnClosed() | Worker [pid:{_processId}] Producer Channel ended by the worker process");
        }

        private void ProducerSocketOnError(Exception? exception)
        {
            _logger.LogDebug(exception, $"ProducerSocketOnError() | Worker [pid:{_processId}] Producer Channel error");
        }

        #endregion Event handles

        #region Private Methods

        private void ProcessMessage(string payload)
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
                    _logger.LogError($"ProcessMessage() | Worker [pid:{_processId}] Received response does not match any sent request [id:{id}], payload:{payload}");
                    return;
                }

                if (accepted.HasValue && accepted.Value)
                {
                    _logger.LogDebug($"ProcessMessage() | Worker [pid:{_processId}] Request succeed [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]");
                    sent.Resolve?.Invoke(data);
                }
                else if (!error.IsNullOrWhiteSpace())
                {
                    // 在 Node.js 实现中，error 的值可能是 "Error" 或 "TypeError"。
                    _logger.LogWarning($"ProcessMessage() | Worker [pid:{_processId}] Request failed [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]: {reason}. payload:{payload}");

                    sent.Reject?.Invoke(new Exception($"Request failed [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]: {reason}. payload:{payload}"));
                }
                else
                {
                    _logger.LogError($"ProcessMessage() | Worker [pid:{_processId}] Received response is not accepted nor rejected [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]. payload:{payload}");

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
                _logger.LogError($"ProcessMessage() | Worker [pid:{_processId}] Received message is not a response nor a notification: {payload}");
            }
        }

        #endregion Private Methods
    }
}
