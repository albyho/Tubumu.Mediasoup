using System;
using FBS.Message;
using Google.FlatBuffers;
using Microsoft.Extensions.Logging;
using Tubumu.Libuv;

namespace Tubumu.Mediasoup
{
    public class Channel : ChannelBase
    {
        #region Constants

        private const int RecvBufferMaxLen = PayloadMaxLen * 2;

        #endregion Constants

        #region Private Fields

        /// <summary>
        /// Unix Socket instance for sending messages to the worker process.
        /// </summary>
        private readonly UVStream _producerSocket;

        /// <summary>
        /// Unix Socket instance for receiving messages to the worker process.
        /// </summary>
        private readonly UVStream _consumerSocket;

        // TODO: CircularBuffer
        /// <summary>
        /// Buffer for reading messages from the worker.
        /// </summary>
        private readonly byte[] _recvBuffer;

        private int _recvBufferCount;

        #endregion Private Fields

        public Channel(ILogger<Channel> logger, UVStream producerSocket, UVStream consumerSocket, int processId)
            : base(logger, processId)
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
            base.Cleanup();

            // Remove event listeners but leave a fake 'error' hander to avoid
            // propagation.
            _consumerSocket.Data -= ConsumerSocketOnData;
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
                _logger.LogError(ex, "CloseAsync() | Worker[{WorkerId}] _producerSocket.Close()", _workerId);
            }

            try
            {
                _consumerSocket.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CloseAsync() | Worker[{WorkerId}] _consumerSocket.Close()", _workerId);
            }
        }

        protected override void SendRequest(Sent sent)
        {
            Loop.Default.Sync(() =>
            {
                try
                {
                    // This may throw if closed or remote side ended.
                    _producerSocket.Write(
                        sent.RequestMessage.Payload,
                        ex =>
                        {
                            if (ex != null)
                            {
                                _logger.LogError(ex, "_producerSocket.Write() | Worker[{WorkerId}] Error", _workerId);
                                sent.Reject(ex);
                            }
                        }
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "_producerSocket.Write() | Worker[{WorkerId}] Error", _workerId);
                    sent.Reject(ex);
                }
            });
        }

        protected override void SendNotification(RequestMessage requestMessage)
        {
            Loop.Default.Sync(() =>
            {
                try
                {
                    // This may throw if closed or remote side ended.
                    _producerSocket.Write(
                        requestMessage.Payload,
                        ex =>
                        {
                            if (ex != null)
                            {
                                _logger.LogError(ex, "_producerSocket.Write() | Worker[{WorkerId}] Error", _workerId);
                            }
                        }
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "_producerSocket.Write() | Worker[{WorkerId}] Error", _workerId);
                }
            });
        }

        #region Event handles

        private void ConsumerSocketOnData(ArraySegment<byte> data)
        {
            // 数据回调通过单一线程进入，所以 _recvBuffer 是 Thread-safe 的。
            if (_recvBufferCount + data.Count > RecvBufferMaxLen)
            {
                _logger.LogError(
                    "ConsumerSocketOnData() | Worker[{WorkerId}] Receiving buffer is full, discarding all data into it",
                    _workerId
                );
                _recvBufferCount = 0;
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

                    var messageBytes = new byte[msgLen];
                    Array.Copy(_recvBuffer, readCount, messageBytes, 0, msgLen);
                    readCount += msgLen;

                    var buf = new ByteBuffer(messageBytes);
                    var message = Message.GetRootAsMessage(buf);
                    ProcessMessage(message);
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
                _logger.LogError(
                    ex,
                    "ConsumerSocketOnData() | Worker[{WorkerId}] Invalid data received from the worker process.",
                    _workerId
                );
            }
        }

        private void ConsumerSocketOnClosed()
        {
            _logger.LogDebug(
                "ConsumerSocketOnClosed() | Worker[{WorkerId}] Consumer Channel ended by the worker process",
                _workerId
            );
        }

        private void ConsumerSocketOnError(Exception? exception)
        {
            _logger.LogDebug(exception, "ConsumerSocketOnError() | Worker[{WorkerId}] Consumer Channel error", _workerId);
        }

        private void ProducerSocketOnClosed()
        {
            _logger.LogDebug(
                "ProducerSocketOnClosed() | Worker[{WorkerId}] Producer Channel ended by the worker process",
                _workerId
            );
        }

        private void ProducerSocketOnError(Exception? exception)
        {
            _logger.LogDebug(exception, "ProducerSocketOnError() | Worker[{WorkerId}] Producer Channel error", _workerId);
        }

        #endregion Event handles
    }
}
