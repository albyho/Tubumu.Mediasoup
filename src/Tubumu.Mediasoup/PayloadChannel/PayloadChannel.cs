using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using Tubumu.Libuv;
using Tubumu.Utils.Extensions;
using Tubumu.Utils.Extensions.Object;
using Tubumu.Utils.Json;
using ObjectExtensions = Tubumu.Utils.Extensions.Object.ObjectExtensions;

namespace Tubumu.Mediasoup
{
    public class PayloadChannel : PayloadChannelBase
    {
        #region Constants

        private const int RecvBufferMaxLen = PayloadMaxLen * 2;

        #endregion Constants

        #region Protected Fields

        /// <summary>
        /// Unix Socket instance for sending messages to the worker process.
        /// </summary>
        private readonly UVStream _producerSocket;

        /// <summary>
        /// Unix Socket instance for receiving messages to the worker process.
        /// </summary>
        private readonly UVStream _consumerSocket;

        #endregion Protected Fields

        #region Private Fields

        /// <summary>
        /// Buffer for reading messages from the worker.
        /// </summary>
        private readonly byte[] _recvBuffer;
        private int _recvBufferCount;

        #endregion

        #region Events

        public override event Action<string, string, NotifyData, ArraySegment<byte>>? MessageEvent;

        #endregion Events

        public PayloadChannel(ILogger<PayloadChannel> logger, UVStream producerSocket, UVStream consumerSocket, int processId) : base(logger, processId)
        {
            _producerSocket = producerSocket;
            _consumerSocket = consumerSocket;

            _recvBuffer = new byte[RecvBufferMaxLen];
            _recvBufferCount = 0;

            _consumerSocket.Data += ConsumerSocketOnData;
            _consumerSocket.Closed += ConsumerSocketOnClosed;
            _consumerSocket.Error += ConsumerSocketOnError;
            _producerSocket.Closed += ProducerSocketOnClosed;
            _producerSocket.Error += ProducerSocketOnError;
        }

        public override void Cleanup()
        {
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
                _logger.LogError(ex, $"CloseAsync() | Worker[{_workerId}]");
            }

            try
            {
                _consumerSocket.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"CloseAsync() | Worker[{_workerId}]");
            }
        }

        protected override void SendNotification(RequestMessage notification)
        {
            var messageJson = notification.ToJson();
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);
            var payloadBytes = notification.Payload!;

            Loop.Default.Sync(() =>
            {
                try
                {
                    var messageBytesLengthBytes = BitConverter.GetBytes(messageBytes.Length);

                    // This may throw if closed or remote side ended.
                    _producerSocket.Write(messageBytesLengthBytes, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_workerId}] Error");
                        }
                    });
                    _producerSocket.Write(messageBytes, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_workerId}] Error");
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"NotifyAsync() | Worker[{_workerId}] Sending notification failed");
                    return;
                }

                try
                {
                    var payloadBytesLengthBytes = BitConverter.GetBytes(payloadBytes.Length);

                    // This may throw if closed or remote side ended.
                    _producerSocket.Write(payloadBytesLengthBytes, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_workerId}] Error");
                        }
                    });
                    _producerSocket.Write(payloadBytes, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_workerId}] Error");
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"NotifyAsync() | Worker[{_workerId}] Sending notification failed");
                    return;
                }
            });
        }

        protected override void SendRequestMessage(RequestMessage requestMessage, Sent sent)
        {
            var requestMessageJson = requestMessage.ToJson();
            var requestMessageBytes = Encoding.UTF8.GetBytes(requestMessageJson);

            if (requestMessageBytes.Length > MessageMaxLen)
            {
                throw new Exception("PayloadChannel message too big");
            }
            else if (requestMessage.Payload != null && requestMessage.Payload.Length > PayloadMaxLen)
            {
                throw new Exception("PayloadChannel payload too big");
            }

            Loop.Default.Sync(() =>
            {
                try
                {
                    var requestMessageBytesLengthBytes = BitConverter.GetBytes(requestMessageBytes.Length);

                    // This may throw if closed or remote side ended.
                    _producerSocket.Write(requestMessageBytesLengthBytes, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_workerId}] Error");
                            sent.Reject(ex);
                        }
                    });
                    // This may throw if closed or remote side ended.
                    _producerSocket.Write(requestMessageBytes, ex =>
                    {
                        if (ex != null)
                        {
                            _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_workerId}] Error");
                            sent.Reject(ex);
                        }
                    });

                    if (requestMessage.Payload != null)
                    {
                        var payloadLengthBytes = BitConverter.GetBytes(requestMessage.Payload.Length);

                        // This may throw if closed or remote side ended.
                        _producerSocket.Write(payloadLengthBytes, ex =>
                        {
                            if (ex != null)
                            {
                                _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_workerId}] Error");
                                sent.Reject(ex);
                            }
                        });

                        // This may throw if closed or remote side ended.
                        _producerSocket.Write(requestMessage.Payload, ex =>
                        {
                            if (ex != null)
                            {
                                _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_workerId}] Error");
                                sent.Reject(ex);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"_producerSocket.Write() | Worker[{_workerId}] Error");
                    sent.Reject(ex);
                }
            });
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
                        Process(payload);
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
                _logger.LogError(ex, $"ConsumerSocketOnData() | Worker[{_workerId}] Invalid data received from the worker process.");
                return;
            }
        }

        private void ConsumerSocketOnClosed()
        {
            _logger.LogDebug($"ConsumerSocketOnClosed() | Worker[{_workerId}] Consumer Channel ended by the worker process");
        }

        private void ConsumerSocketOnError(Exception? exception)
        {
            _logger.LogDebug(exception, $"ConsumerSocketOnError() | Worker[{_workerId}] Consumer Channel error");
        }

        private void ProducerSocketOnClosed()
        {
            _logger.LogDebug($"ProducerSocketOnClosed() | Worker[{_workerId}] Producer Channel ended by the worker process");
        }

        private void ProducerSocketOnError(Exception? exception)
        {
            _logger.LogDebug(exception, $"ProducerSocketOnError() | Worker[{_workerId}] Producer Channel error");
        }

        #endregion Event handles

        #region Process Methods

        public override void Process(string message, byte[] payload)
        {
            throw new NotImplementedException();
        }

        private void Process(byte[] payload)
        {
            if (_ongoingNotification == null)
            {
                var message = Encoding.UTF8.GetString(payload, 0, payload.Length);
                var jsonDocument = JsonDocument.Parse(message);
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
                        _logger.LogError($"ProcessData() | Worker[{_workerId}] Received response does not match any sent request [id:{id}]");
                        return;
                    }

                    if (accepted.HasValue && accepted.Value)
                    {
                        _logger.LogDebug($"ProcessData() | Worker[{_workerId}] Request succeed [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]");

                        sent.Resolve?.Invoke(data);
                    }
                    else if (!error.IsNullOrWhiteSpace())
                    {
                        // 在 Node.js 实现中，error 的值可能是 "Error" 或 "TypeError"。
                        _logger.LogWarning($"ProcessData() | Worker[{_workerId}] Request failed [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]: {reason}");

                        sent.Reject?.Invoke(new Exception(reason));
                    }
                    else
                    {
                        _logger.LogError($"ProcessData() | Worker[{_workerId}] Received response is not accepted nor rejected [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]");
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
                    _logger.LogError($"ProcessData() | Worker[{_workerId}] Received data is not a notification nor a response");
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
