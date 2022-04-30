using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using Tubumu.Utils.Extensions;
using Tubumu.Utils.Extensions.Object;

namespace Tubumu.Mediasoup
{
    public abstract class PayloadChannelBase : IPayloadChannel
    {
        #region Constants

        protected const int MessageMaxLen = PayloadMaxLen + sizeof(int);

        protected const int PayloadMaxLen = 1024 * 1024 * 4;

        #endregion Constants

        #region Protected Fields

        /// <summary>
        /// Logger.
        /// </summary>
        protected readonly ILogger<PayloadChannel> _logger;

        /// <summary>
        /// Closed flag.
        /// </summary>
        protected bool _closed;

        /// <summary>
        /// Close locker.
        /// </summary>
        protected readonly AsyncAutoResetEvent _closeLock = new();

        /// <summary>
        /// Worker process PID.
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

        /// <summary>
        /// Ongoing notification (waiting for its payload).
        /// </summary>
        protected OngoingNotification? _ongoingNotification;

        #endregion Protected Fields

        #region Events

        public abstract event Action<string, string, NotifyData, ArraySegment<byte>>? MessageEvent;

        #endregion Events

        public PayloadChannelBase(ILogger<PayloadChannel> logger, int processId)
        {
            _logger = logger;
            _closeLock.Set();
            _workerId = processId;
        }

        public virtual void Cleanup()
        {

        }

        public async Task CloseAsync()
        {
            if (_closed)
            {
                return;
            }

            await _closeLock.WaitAsync();
            try
            {
                if (_closed)
                {
                    return;
                }

                _logger.LogDebug($"CloseAsync() | Worker[{_workerId}]");

                _closed = true;

                Cleanup();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"CloseAsync() | Worker[{_workerId}]");
            }
            finally
            {
                _closeLock.Set();
            }
        }

        protected abstract void SendNotification(RequestMessage notification);
        protected abstract void SendRequestMessage(RequestMessage requestMessage, Sent sent);

        private RequestMessage CreateRequestMessage(MethodId methodId, object? @internal = null, object? data = null, byte[]? payload = null)
        {
            var id = InterlockedExtensions.Increment(ref _nextId);
            var method = methodId.GetEnumMemberValue();

            var requestMesssge = new RequestMessage
            {
                Id = id,
                Method = method,
                Internal = @internal,
                Data = data,
                Payload = payload,
            };

            return requestMesssge;
        }

        private RequestMessage CreateNotification(string @event, object @internal, NotifyData? data, byte[] payload)
        {
            var notification = new RequestMessage
            {
                Event = @event,
                Internal = @internal,
                Data = data,
                Payload = payload,
            };

            return notification;
        }

        public async Task NotifyAsync(string @event, object @internal, NotifyData? data, byte[] payload)
        {
            if (_closed)
            {
                throw new InvalidStateException("PayloadChannel closed");
            }

            await _closeLock.WaitAsync();
            try
            {
                if (_closed)
                {
                    return;
                }

                _logger.LogDebug($"NotifyAsync() | Worker[{_workerId}] Event:{@event}");

                var notification = CreateNotification(@event, @internal, data, payload);
                SendNotification(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NotifyAsync()");
            }
            finally
            {
                _closeLock.Set();
            }
        }

        public async Task<string?> RequestAsync(MethodId methodId, object? @internal = null, object? data = null, byte[]? payload = null)
        {
            if (_closed)
            {
                throw new InvalidStateException("Channel closed");
            }

            await _closeLock.WaitAsync();
            try
            {
                if (_closed)
                {
                    return null;
                }


                var requestMessage = CreateRequestMessage(methodId, @internal, data);

                _logger.LogDebug($"RequestAsync() | Worker[{_workerId}] Method:{requestMessage.Method}");

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "RequestAsync()");
            }
            finally
            {
                _closeLock.Set();
            }

            return null;
        }

        #region Process Methods

        public abstract void Process(string message, byte[] payload);

        #endregion Process Methods
    }
}
