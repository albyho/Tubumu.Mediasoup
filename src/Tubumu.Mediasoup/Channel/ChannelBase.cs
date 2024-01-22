using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FBS.Log;
using FBS.Message;
using FBS.Notification;
using FBS.Request;
using FBS.Response;
using Google.FlatBuffers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

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
        protected readonly ILogger<ChannelBase> _logger;

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

        public event Action<string, Event, Notification>? OnNotification;

        #endregion Events

        protected ChannelBase(ILogger<ChannelBase> logger, int workerId)
        {
            _logger = logger;
            _workerId = workerId;
        }

        public async Task CloseAsync()
        {
            _logger.LogDebug("CloseAsync() | Worker[{WorkId}]", _workerId);

            using(await _closeLock.WriteLockAsync())
            {
                if(_closed)
                {
                    return;
                }

                _closed = true;

                Cleanup();
            }
        }

        public virtual void Cleanup()
        {
            // Close every pending sent.
            try
            {
                _sents.Values.ForEach(m => m.Close());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Cleanup() | Worker[{WorkId}] _sents.Values.ForEach(m => m.Close.Invoke())", _workerId);
            }
        }

        public async Task NotifyAsync(FlatBufferBuilder bufferBuilder, Event @event, FBS.Notification.Body? bodyType, int? bodyOffset, string? handlerId)
        {
            _logger.LogDebug("NotifyAsync() | Worker[{WorkId}] Event:{Event}", _workerId, @event);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("PayloadChannel closed");
                }

                var notificationRequestMessage = CreateNotificationRequestMessage(bufferBuilder, @event, bodyType, bodyOffset, handlerId);
                SendNotification(notificationRequestMessage);
            }
        }

        protected abstract void SendNotification(RequestMessage requestMessage);

        public async Task<Response?> RequestAsync(FlatBufferBuilder bufferBuilder, Method method, FBS.Request.Body? bodyType = null, int? bodyOffset = null, string? handlerId = null)
        {
            _logger.LogDebug("RequestAsync() | Worker[{WorkId}] Method:{Method}", _workerId, method);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Channel closed");
                }

                var requestMessage = CreateRequestRequestMessage(bufferBuilder, method, bodyType, bodyOffset, handlerId);

                var tcs = new TaskCompletionSource<Response?>();
                var sent = new Sent
                {
                    RequestMessage = requestMessage,
                    Resolve = data =>
                    {
                        if(!_sents.TryRemove(requestMessage.Id!.Value, out _))
                        {
                            tcs.TrySetException(
                                new Exception($"Received response does not match any sent request [id:{requestMessage.Id}]")
                            );
                            return;
                        }

                        tcs.TrySetResult(data);
                    },
                    Reject = e =>
                    {
                        if(!_sents.TryRemove(requestMessage.Id!.Value, out _))
                        {
                            tcs.TrySetException(
                                new Exception($"Received response does not match any sent request [id:{requestMessage.Id}]")
                            );
                            return;
                        }

                        tcs.TrySetException(e);
                    },
                    Close = () => tcs.TrySetException(new InvalidStateException("Channel closed"))
                };
                if(!_sents.TryAdd(requestMessage.Id!.Value, sent))
                {
                    throw new Exception($"Error add sent request [id:{requestMessage.Id}]");
                }

                tcs.WithTimeout(
                    TimeSpan.FromSeconds(15 + (0.1 * _sents.Count)),
                    () => _sents.TryRemove(requestMessage.Id!.Value, out _)
                );

                SendRequest(sent);

                return await tcs.Task;
            }
        }

        protected abstract void SendRequest(Sent sent);

        #region Event handles

        public void ProcessMessage(Message message)
        {
            try
            {
                switch(message.DataType)
                {
                    case FBS.Message.Body.Response:
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            var response = message.DataAsResponse();
                            ProcessResponse(response);
                        });
                        break;
                    case FBS.Message.Body.Notification:
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            var notification = message.DataAsNotification();
                            ProcessNotification(notification);
                        });
                        break;
                    case FBS.Message.Body.Log:
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            var log = message.DataAsLog();
                            ProcessLog(log);
                        });
                        break;
                    default:
                        {
                            _logger.LogWarning("ProcessMessage() | worker[WorkerId/Pid:{WorkerId}] unexpected", _workerId);
                        }
                        break;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "ProcessMessage() | Worker[{WorkerId}] Received invalid message from the worker process", _workerId);
                return;
            }
        }

        private void ProcessResponse(Response response)
        {
            if(!_sents.TryGetValue(response.Id, out var sent))
            {
                _logger.LogError(
                    "ProcessResponse() | Worker[{WorkerId}] Received response does not match any sent request [id:{Id}]",
                    _workerId,
                    response.Id
                );
                return;
            }

            if(response.Accepted)
            {
                _logger.LogDebug(
                    "ProcessResponse() | Worker[{WorkerId}] Request succeed [method:{Method}, id:{Id}]",
                    _workerId,
                    sent.RequestMessage.Method,
                    response.Id
                );
                sent.Resolve(response);
            }
            else if(!response.Error.IsNullOrWhiteSpace())
            {
                // 在 Node.js 实现中，error 的值可能是 "Error" 或 "TypeError"。
                _logger.LogWarning(
                    "ProcessResponse() | Worker[{WorkerId}] Request failed [method:{Method}, id:{Id}, reason:{Reson}]",
                    _workerId,
                    sent.RequestMessage.Method,
                    response.Reason,
                    response.Id
                );

                sent.Reject(
                    new Exception(
                        $"Request failed [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}, reason:{response.Reason}]"
                    )
                );
            }
            else
            {
                _logger.LogError(
                    "ProcessResponse() | Worker[{WorkerId}] Received response is not accepted nor rejected [method:{Method}, id:{Id}]",
                    _workerId,
                    sent.RequestMessage.Method,
                    response.Id
                );

                sent.Reject(
                    new Exception(
                        $"Received response is not accepted nor rejected [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]"
                    )
                );
            }
        }

        private void ProcessNotification(Notification notification)
        {
            OnNotification?.Invoke(notification.HandlerId, notification.Event, notification);
        }

        private void ProcessLog(Log log)
        {
            var logData = log.Data;

            switch(logData[0])
            {
                // 'D' (a debug log).
                case 'D':
                    {
                        _logger.LogDebug("[WorkerId/Pid:{WorkerId}] {Flag}", _workerId, logData[1..]);

                        break;
                    }

                // 'W' (a warn log).
                case 'W':
                    {
                        _logger.LogWarning("[WorkerId/Pid:{WorkerId}] {Flag}", _workerId, logData[1..]);

                        break;
                    }

                // 'E' (a error log).
                case 'E':
                    {
                        _logger.LogError("[WorkerId/Pid:{WorkerId}] {Flag}", _workerId, logData[1..]);

                        break;
                    }

                // 'X' (a dump log).
                case 'X':
                    {
                        // eslint-disable-next-line no-console
                        _logger.LogTrace("[WorkerId/Pid:{WorkerId}] {Flag}", _workerId, logData[1..]);

                        break;
                    }
            }
        }

        #endregion Event handles

        private RequestMessage CreateRequestRequestMessage(
            FlatBufferBuilder bufferBuilder,
            Method method,
            FBS.Request.Body? bodyType,
            int? bodyOffset,
            string? handlerId
        )
        {
            var id = InterlockedExtensions.Increment(ref _nextId);

            var handlerIdOffset = bufferBuilder.CreateString(handlerId ?? "");

            Offset<Request> requestOffset;

            if(bodyType.HasValue && bodyOffset.HasValue)
            {
                requestOffset = Request.CreateRequest(
                    bufferBuilder,
                    id,
                    method,
                    handlerIdOffset,
                    bodyType.Value,
                    bodyOffset.Value
                );
            }
            else
            {
                requestOffset = Request.CreateRequest(bufferBuilder, id, method, handlerIdOffset, FBS.Request.Body.NONE, 0);
            }

            var messageOffset = Message.CreateMessage(bufferBuilder, FBS.Message.Body.Request, requestOffset.Value);

            // Finalizes the buffer and adds a 4 byte prefix with the size of the buffer.
            bufferBuilder.FinishSizePrefixed(messageOffset.Value);

            // Create a new buffer with this data so multiple contiguous flatbuffers
            // do not point to the builder buffer overriding others info.
            //var buffer = bufferBuilder.DataBuffer.ToSizedArray();

            // Zero copy.
            var buffer = bufferBuilder.DataBuffer.ToArraySegment(bufferBuilder.DataBuffer.Position, bufferBuilder.DataBuffer.Length - bufferBuilder.DataBuffer.Position);

            // Clear the buffer builder so it's reused for the next request.
            //bufferBuilder.Clear();

            if(buffer.Count > MessageMaxLen)
            {
                throw new Exception($"request too big [method:{method}]");
            }

            var requestMessage = new RequestMessage
            {
                Id = id,
                Method = method,
                HandlerId = handlerId,
                Payload = buffer
            };
            return requestMessage;
        }

        private static RequestMessage CreateNotificationRequestMessage(
            FlatBufferBuilder bufferBuilder,
            Event @event,
            FBS.Notification.Body? bodyType,
            int? bodyOffset,
            string? handlerId
        )
        {
            var handlerIdOffset = bufferBuilder.CreateString(handlerId ?? "");

            Offset<Notification> notificationOffset;

            if(bodyType.HasValue && bodyOffset.HasValue)
            {
                notificationOffset = Notification.CreateNotification(
                    bufferBuilder,
                    handlerIdOffset,
                    @event,
                    bodyType.Value,
                    bodyOffset.Value
                );
            }
            else
            {
                notificationOffset = Notification.CreateNotification(
                    bufferBuilder,
                    handlerIdOffset,
                    @event,
                    FBS.Notification.Body.NONE,
                    0
                );
            }

            var messageOffset = Message.CreateMessage(bufferBuilder, FBS.Message.Body.Notification, notificationOffset.Value);

            // Finalizes the buffer and adds a 4 byte prefix with the size of the buffer.
            bufferBuilder.FinishSizePrefixed(messageOffset.Value);

            // Create a new buffer with this data so multiple contiguous flatbuffers
            // do not point to the builder buffer overriding others info.
            //var buffer = bufferBuilder.DataBuffer.ToSizedArray();

            // Zero copy.
            var buffer = bufferBuilder.DataBuffer.ToArraySegment(bufferBuilder.DataBuffer.Position, bufferBuilder.DataBuffer.Length - bufferBuilder.DataBuffer.Position);

            // Clear the buffer builder so it's reused for the next request.
            //BufferBuilder.Clear();

            if(buffer.Count > MessageMaxLen)
            {
                throw new Exception($"notification too big [event:{@event}]");
            }

            var requestMessage = new RequestMessage
            {
                Event = @event,
                HandlerId = handlerId,
                Payload = buffer
            };
            return requestMessage;
        }
    }
}
