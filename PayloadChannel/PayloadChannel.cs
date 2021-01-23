using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tubumu.Core.Extensions;
using Tubumu.Core.Extensions.Object;
using TubumuMeeting.Core;
using Tubumu.Libuv;

namespace TubumuMeeting.Mediasoup
{
    public class PayloadChannel
    {
        #region Constants

        private const int NsMessageMaxLen = 4194313;

        private const int NsPayloadMaxLen = 4194304;

        #endregion Constants

        #region Private Fields

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<PayloadChannel> _logger;

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
        private readonly ConcurrentDictionary<uint, Sent> _sents = new ConcurrentDictionary<uint, Sent>();

        /// <summary>
        /// Buffer for reading messages from the worker.
        /// </summary>
        private ArraySegment<byte>? _recvBuffer;

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

            _logger.LogDebug($"Close() | PayloadChannel");

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
            catch (Exception)
            {
            }

            try
            {
                _consumerSocket.Close();
            }
            catch (Exception)
            {
            }
        }

        public void Notify(string @event, object @internal, NotifyData? data, byte[] payload)
        {
            _logger.LogDebug($"Notify() [Event:{@event}]");

            if (_closed)
            {
                throw new InvalidStateException("PayloadChannel closed");
            }

            var notification = new { @event, @internal, data };
            var ns1Bytes = Netstring.Encode(notification.ToCamelCaseJson());
            var ns2Bytes = Netstring.Encode(payload);

            if (ns1Bytes.Length > NsMessageMaxLen)
            {
                throw new Exception("PayloadChannel notification too big");
            }
            if (ns2Bytes.Length > NsMessageMaxLen)
            {
                throw new Exception("PayloadChannel payload too big");
            }

            Loop.Default.Sync(() =>
            {
                try
                {
                    // This may throw if closed or remote side ended.
                    _producerSocket.Write(ns1Bytes, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, "_producerSocket.Write() | Error");
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Notify() | Sending notification failed");
                    return;
                }

                try
                {
                    // This may throw if closed or remote side ended.
                    _producerSocket.Write(ns2Bytes, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, "_producerSocket.Write() | Error");
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Notify() | sending notification failed");
                    return;
                }
            });
        }

        public Task<string?> RequestAsync(MethodId methodId, object? @internal = null, object? data = null, byte[]? payload = null)
        {
            var method = methodId.GetEnumStringValue();
            var id = InterlockedExtensions.Increment(ref _nextId);

            _logger.LogDebug($"RequestAsync() | [Method:{method}, Id:{id}]");

            if (_closed)
            {
                throw new InvalidStateException("Channel closed");
            }

            var requestMesssge = new RequestMessage
            {
                Id = id,
                Method = method,
                Internal = @internal,
                Data = data,
            };
            var nsBytes1 = Netstring.Encode(requestMesssge.ToCamelCaseJson());
            var nsBytes2 = Netstring.Encode(payload);
            if (nsBytes1.Length > NsMessageMaxLen)
            {
                throw new Exception("Channel request too big");
            }
            else if (nsBytes2.Length > NsMessageMaxLen)
            {
                throw new Exception("PayloadChannel request too big");
            }

            var tcs = new TaskCompletionSource<string?>();

            var sent = new Sent
            {
                RequestMessage = requestMesssge,
                Resolve = data =>
                {
                    if (!_sents.TryRemove(id, out var _))
                    {
                        tcs.TrySetException(new Exception($"Received response does not match any sent request [id:{id}]"));
                        return;
                    }
                    tcs.TrySetResult(data);
                },
                Reject = e =>
                {
                    if (!_sents.TryRemove(id, out var _))
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

            tcs.WithTimeout(TimeSpan.FromSeconds(15 + (0.1 * _sents.Count)), () => _sents.TryRemove(id, out var _));

            Loop.Default.Sync(() =>
            {
                try
                {
                    // This may throw if closed or remote side ended.
                    _producerSocket.Write(nsBytes1, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, "_producerSocket.Write() | Error");
                            sent.Reject(ex);
                        }
                    });
                    _producerSocket.Write(nsBytes2, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, "_producerSocket.Write() | Error");
                            sent.Reject(ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "_producerSocket.Write() | Error");
                    sent.Reject(ex);
                }
            });

            return tcs.Task;
        }

        #region Event handles

        private void ConsumerSocketOnData(ArraySegment<byte> data)
        {
            if (_recvBuffer == null)
            {
                _recvBuffer = data;
            }
            else
            {
                var newBuffer = new byte[_recvBuffer.Value.Count + data.Count];
                Array.Copy(_recvBuffer.Value.Array, _recvBuffer.Value.Offset, newBuffer, 0, _recvBuffer.Value.Count);
                Array.Copy(data.Array, data.Offset, newBuffer, _recvBuffer.Value.Count, data.Count);
                _recvBuffer = new ArraySegment<byte>(newBuffer);
            }

            if (_recvBuffer.Value.Count > NsPayloadMaxLen)
            {
                _logger.LogError("ConsumerSocketOnData() | Receiving buffer is full, discarding all data into it");
                // Reset the buffer and exit.
                _recvBuffer = null;
                return;
            }

            //_logger.LogError($"ConsumerSocketOnData: {buffer}");
            var netstring = new Netstring(_recvBuffer.Value);
            try
            {
                var nsLength = 0;
                foreach (var payload in netstring)
                {
                    nsLength += payload.NetstringLength;
                    ProcessData(payload);
                }

                if (nsLength > 0)
                {
                    if (nsLength == _recvBuffer.Value.Count)
                    {
                        // Reset the buffer.
                        _recvBuffer = null;
                    }
                    else
                    {
                        _recvBuffer = new ArraySegment<byte>(_recvBuffer.Value.Array, _recvBuffer.Value.Offset + nsLength, _recvBuffer.Value.Count - nsLength);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ConsumerSocketOnData() | Invalid netstring data received from the worker process.");
                // Reset the buffer and exit.
                _recvBuffer = null;
                return;
            }
        }

        private void ConsumerSocketOnClosed()
        {
            _logger.LogDebug("ConsumerSocketOnClosed() | Consumer Channel ended by the worker process");
        }

        private void ConsumerSocketOnError(Exception exception)
        {
            _logger.LogDebug(exception, "ConsumerSocketOnError() | Consumer Channel error");
        }

        private void ProducerSocketOnClosed()
        {
            _logger.LogDebug("ProducerSocketOnClosed() | Producer Channel ended by the worker process");
        }

        private void ProducerSocketOnError(Exception exception)
        {
            _logger.LogDebug(exception, "ProducerSocketOnError() | Producer Channel error");
        }

        #endregion Event handles

        #region Private Methods

        private void ProcessData(Payload payload)
        {
            if (_ongoingNotification == null)
            {
                var payloadString = Encoding.UTF8.GetString(payload.Data.Array, payload.Data.Offset, payload.Data.Count);
                var msg = JObject.Parse(payloadString);
                var id = msg["id"].Value((uint)0);
                var accepted = msg["accepted"].Value(false);
                var targetId = msg["targetId"].Value(String.Empty);
                var @event = msg["event"].Value(string.Empty);
                var error = msg["error"].Value(string.Empty);
                var reason = msg["reason"].Value(string.Empty);
                var data = msg["data"].Value(string.Empty);

                // If a response, retrieve its associated request.
                if (id > 0)
                {
                    if (!_sents.TryGetValue(id, out Sent sent))
                    {
                        _logger.LogError($"ProcessData() | Received response does not match any sent request [id:{id}]");

                        return;
                    }

                    if (accepted)
                    {
                        _logger.LogDebug($"ProcessData() | Request succeed [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]");

                        sent.Resolve?.Invoke(data);
                    }
                    else if (!error.IsNullOrWhiteSpace())
                    {
                        // 在 Node.js 实现中，error 的值可能是 "Error" 或 "TypeError"。
                        _logger.LogWarning($"ProcessData() | Request failed [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]: {reason}");

                        sent.Reject?.Invoke(new Exception(reason));
                    }
                    else
                    {
                        _logger.LogError($"ProcessData() | Received response is not accepted nor rejected [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]");
                    }
                }
                // If a notification emit it to the corresponding entity.
                else if (!targetId.IsNullOrWhiteSpace() && !@event.IsNullOrWhiteSpace())
                {
                    var notifyData = JsonConvert.DeserializeObject<NotifyData>(data);
                    _ongoingNotification = new OngoingNotification
                    {
                        TargetId = targetId,
                        Event = @event,
                        Data = notifyData,
                    };
                }
                else
                {
                    _logger.LogError("ProcessData() | Received data is not a notification nor a response");
                    return;
                }
            }
            else
            {
                // Emit the corresponding event.
                MessageEvent?.Invoke(_ongoingNotification.TargetId, _ongoingNotification.Event, _ongoingNotification.Data, payload.Data);

                // Unset ongoing notification.
                _ongoingNotification = null;
            }
        }

        #endregion Private Methods
    }
}
