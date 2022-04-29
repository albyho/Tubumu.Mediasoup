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
using ObjectExtensions = Tubumu.Utils.Extensions.Object.ObjectExtensions;

namespace Tubumu.Mediasoup
{
    public class PayloadChannel : IPayloadChannel
    {
        #region Constants

        protected const int MessageMaxLen = 4194308;

        protected const int PayloadMaxLen = 4194304;

        private const int RecvBufferMaxLen = 4194308 * 2;

        #endregion Constants

        #region Private Fields

        /// <summary>
        /// Logger.
        /// </summary>
        protected readonly ILogger<PayloadChannel> _logger;

        // TODO: (alby) _closed 的使用及线程安全。
        /// <summary>
        /// Closed flag.
        /// </summary>
        protected bool _closed;

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
        protected uint _nextId = 0;

        /// <summary>
        /// Map of pending sent requests.
        /// </summary>
        private readonly ConcurrentDictionary<uint, Sent> _sents = new();

        /// <summary>
        /// Buffer for reading messages from the worker.
        /// </summary>
        private readonly byte[] _recvBuffer;
        private int _recvBufferCount;

        /// <summary>
        /// Ongoing notification (waiting for its payload).
        /// </summary>
        private OngoingNotification? _ongoingNotification;

        #endregion Private Fields

        #region Events

        public event Action<string, string, NotifyData, ArraySegment<byte>>? MessageEvent;

        #endregion Events

        public PayloadChannel(ILogger<PayloadChannel> logger, UVStream producerSocket, UVStream consumerSocket, int processId)
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

            _logger.LogDebug($"Close() | Worker[{_processId}]");

            _closed = true;

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
                _logger.LogError(ex, $"Close() | Worker[{_processId}]");
            }

            try
            {
                _consumerSocket.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Close() | Worker[{_processId}]");
            }
        }

        public void Notify(string @event, object @internal, NotifyData? data, byte[] payload)
        {
            _logger.LogDebug($"Notify() | Worker[{_processId}] Event:{@event}");

            if (_closed)
            {
                throw new InvalidStateException("PayloadChannel closed");
            }

            var notification = new { @event, @internal, data };
            var notificationJson = notification.ToJson();
            var notificationBytes = Encoding.UTF8.GetBytes(notificationJson);
            var notificationBytesLengthBytes = BitConverter.GetBytes(notificationBytes.Length);

            if (notificationBytes.Length > MessageMaxLen)
            {
                throw new Exception("PayloadChannel notification too big");
            }
            else if (payload.Length > MessageMaxLen)
            {
                throw new Exception("PayloadChannel payload too big");
            }
            var payloadLengthBytes = BitConverter.GetBytes(payload.Length);

            Loop.Default.Sync(() =>
            {
                try
                {
                    // This may throw if closed or remote side ended.
                    _producerSocket.Write(notificationBytesLengthBytes, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_processId}] Error");
                        }
                    });
                    _producerSocket.Write(notificationBytes, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_processId}] Error");
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Notify() | Worker[{_processId}] Sending notification failed");
                    return;
                }

                try
                {
                    // This may throw if closed or remote side ended.
                    _producerSocket.Write(payloadLengthBytes, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_processId}] Error");
                        }
                    });
                    _producerSocket.Write(payload, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_processId}] Error");
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Notify() | Worker[{_processId}] Sending notification failed");
                    return;
                }
            });
        }

        public Task<string?> RequestAsync(MethodId methodId, object? @internal = null, object? data = null, byte[]? payload = null)
        {
            var method = methodId.GetEnumMemberValue();
            var id = InterlockedExtensions.Increment(ref _nextId);

            _logger.LogDebug($"RequestAsync() | Worker[{_processId}] Method:{method}");

            if (_closed)
            {
                throw new InvalidStateException("Channel closed");
            }

            var requestMessage = new RequestMessage
            {
                Id = id,
                Method = method,
                Internal = @internal,
                Data = data,
            };
            var requestMessageJson = requestMessage.ToJson();
            var requestMessageBytes = Encoding.UTF8.GetBytes(requestMessageJson);
            var requestMessageBytesLengthBytes = BitConverter.GetBytes(requestMessageBytes.Length);

            if (requestMessageBytes.Length > MessageMaxLen)
            {
                throw new Exception("PayloadChannel notification too big");
            }
            else if (payload != null && payload.Length > PayloadMaxLen)
            {
                throw new Exception("PayloadChannel payload too big");
            }

            var tcs = new TaskCompletionSource<string?>();

            var sent = new Sent
            {
                RequestMessage = requestMessage,
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
                    _producerSocket.Write(requestMessageBytesLengthBytes, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_processId}] Error");
                            sent.Reject(ex);
                        }
                    });
                    _producerSocket.Write(requestMessageBytes, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_processId}] Error");
                            sent.Reject(ex);
                        }
                    });

                    if (payload != null)
                    {
                        var payloadLengthBytes = BitConverter.GetBytes(payload.Length);

                        _producerSocket.Write(payloadLengthBytes, ex =>
                        {
                            if (ex != null)
                            {
                                _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_processId}] Error");
                                sent.Reject(ex);
                            }
                        });
                        _producerSocket.Write(payload, ex =>
                        {
                            if (ex != null)
                            {
                                _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_processId}] Error");
                                sent.Reject(ex);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_processId}] Error");
                    sent.Reject(ex);
                }
            });

            return tcs.Task;
        }

        #region Event handles

        private void ConsumerSocketOnData(ArraySegment<byte> data)
        {
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

                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        ProcessData(payload);
                    });
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
                _logger.LogError(ex, $"ConsumerSocketOnData() | Worker[{_processId}] Invalid data received from the worker process.");
                return;
            }
        }

        private void ConsumerSocketOnClosed()
        {
            _logger.LogDebug($"ConsumerSocketOnClosed() | Worker[{_processId}] Consumer Channel ended by the worker process");
        }

        private void ConsumerSocketOnError(Exception? exception)
        {
            _logger.LogDebug(exception, $"ConsumerSocketOnError() | Worker[{_processId}] Consumer Channel error");
        }

        private void ProducerSocketOnClosed()
        {
            _logger.LogDebug($"ProducerSocketOnClosed() | Worker[{_processId}] Producer Channel ended by the worker process");
        }

        private void ProducerSocketOnError(Exception? exception)
        {
            _logger.LogDebug(exception, $"ProducerSocketOnError() | Worker[{_processId}] Producer Channel error");
        }

        #endregion Event handles

        #region Private Methods

        private void ProcessData(byte[] payload)
        {
            if (_ongoingNotification == null)
            {
                var payloadString = Encoding.UTF8.GetString(payload, 0, payload.Length);
                var jsonDocument = JsonDocument.Parse(payloadString);
                var msg = jsonDocument.RootElement;
                var id = msg.GetNullableJsonElement("id")?.GetNullableUInt32();
                var accepted = msg.GetNullableJsonElement("accepted")?.GetNullableBool();
                // targetId 可能是 Number 或 String。不能使用 GetString()，否则可能报错：Cannot get the value of a token type 'Number' as a string"
                var targetId = msg.GetNullableJsonElement("targetId")?.ToString();
                var @event = msg.GetNullableJsonElement("event")?.GetString();
                var error = msg.GetNullableJsonElement("error")?.GetString();
                var reason = msg.GetNullableJsonElement("reason")?.GetString();
                var data = msg.GetNullableJsonElement("data")?.GetString();

                // If a response, retrieve its associated request.
                if (id.HasValue && id.Value >= 0)
                {
                    if (!_sents.TryGetValue(id.Value, out var sent))
                    {
                        _logger.LogError($"ProcessData() | Worker[{_processId}] Received response does not match any sent request [id:{id}]");

                        return;
                    }

                    if (accepted.HasValue && accepted.Value)
                    {
                        _logger.LogDebug($"ProcessData() | Worker[{_processId}] Request succeed [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]");

                        sent.Resolve?.Invoke(data);
                    }
                    else if (!error.IsNullOrWhiteSpace())
                    {
                        // 在 Node.js 实现中，error 的值可能是 "Error" 或 "TypeError"。
                        _logger.LogWarning($"ProcessData() | Worker[{_processId}] Request failed [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]: {reason}");

                        sent.Reject?.Invoke(new Exception(reason));
                    }
                    else
                    {
                        _logger.LogError($"ProcessData() | Worker[{_processId}] Received response is not accepted nor rejected [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]");
                    }
                }
                // If a notification emit it to the corresponding entity.
                else if (!targetId.IsNullOrWhiteSpace() && !@event.IsNullOrWhiteSpace())
                {
                    var notifyData = JsonSerializer.Deserialize<NotifyData>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;
                    _ongoingNotification = new OngoingNotification
                    {
                        TargetId = targetId!,
                        Event = @event!,
                        Data = notifyData,
                    };
                }
                else
                {
                    _logger.LogError($"ProcessData() | Worker[{_processId}] Received data is not a notification nor a response");
                    return;
                }
            }
            else
            {
                // Emit the corresponding event.
                MessageEvent?.Invoke(_ongoingNotification.TargetId, _ongoingNotification.Event, _ongoingNotification.Data, new ArraySegment<byte>(payload));

                // Unset ongoing notification.
                _ongoingNotification = null;
            }
        }

        #endregion Private Methods
    }
}
