using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FBS.Notification;
using FBS.Request;
using Google.FlatBuffers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

namespace Tubumu.Mediasoup
{
    public abstract class RtpObserver : EventEmitter
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<RtpObserver> _logger;

        /// <summary>
        /// Whether the Producer is closed.
        /// </summary>
        private bool _closed;

        private readonly AsyncReaderWriterLock _closeLock = new();

        /// <summary>
        /// Paused flag.
        /// </summary>
        private bool _paused;

        private readonly AsyncAutoResetEvent _pauseLock = new();

        /// <summary>
        /// Internal data.
        /// </summary>
        public RtpObserverInternal Internal { get; }

        /// <summary>
        /// Channel instance.
        /// </summary>
        protected readonly IChannel Channel;

        /// <summary>
        /// App custom data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; }

        /// <summary>
        /// Method to retrieve a Producer.
        /// </summary>
        protected readonly Func<string, Task<Producer?>> GetProducerById;

        /// <summary>
        /// Observer instance.
        /// </summary>
        public EventEmitter Observer { get; } = new EventEmitter();

        /// <summary>
        /// <para>Events:</para>
        /// <para>@emits routerclose</para>
        /// <para>@emits @close</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// <para>@emits pause</para>
        /// <para>@emits resume</para>
        /// <para>@emits addproducer - (producer: Producer)</para>
        /// <para>@emits removeproducer - (producer: Producer)</para>
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="internal_"></param>
        /// <param name="channel"></param>
        /// <param name="appData"></param>
        /// <param name="getProducerById"></param>
        protected RtpObserver(
            ILoggerFactory loggerFactory,
            RtpObserverInternal internal_,
            IChannel channel,
            Dictionary<string, object>? appData,
            Func<string, Task<Producer?>> getProducerById
        )
        {
            _logger = loggerFactory.CreateLogger<RtpObserver>();

            Internal = internal_;
            Channel = channel;
            AppData = appData ?? new Dictionary<string, object>();
            GetProducerById = getProducerById;
            _pauseLock.Set();

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the RtpObserver.
        /// </summary>
        public async Task CloseAsync()
        {
            _logger.LogDebug("Close() | RtpObserverId:{RtpObserverId}", Internal.RtpObserverId);

            using(await _closeLock.WriteLockAsync())
            {
                if(_closed)
                {
                    return;
                }

                _closed = true;

                // Remove notification subscriptions.
                Channel.OnNotification -= OnNotificationHandle;

                // Build Request
                var bufferBuilder = Channel.BufferPool.Get();

                var closeRtpObserverRequest = new FBS.Router.CloseRtpObserverRequestT
                {
                    RtpObserverId = Internal.RtpObserverId,
                };

                var closeRtpObserverRequestOffset = FBS.Router.CloseRtpObserverRequest.Pack(bufferBuilder, closeRtpObserverRequest);

                // Fire and forget
                Channel.RequestAsync(bufferBuilder, Method.ROUTER_CLOSE_RTPOBSERVER,
                    FBS.Request.Body.Router_CloseRtpObserverRequest,
                    closeRtpObserverRequestOffset.Value,
                    Internal.RouterId
                    ).ContinueWithOnFaultedHandleLog(_logger);

                Emit("@close");

                // Emit observer event.
                Observer.Emit("close");
            }
        }

        /// <summary>
        /// Router was closed.
        /// </summary>
        public async Task RouterClosedAsync()
        {
            _logger.LogDebug("RouterClosed() | RtpObserverId:{RtpObserverId}", Internal.RtpObserverId);

            using(await _closeLock.WriteLockAsync())
            {
                if(_closed)
                {
                    return;
                }

                _closed = true;

                // Remove notification subscriptions.
                Channel.OnNotification -= OnNotificationHandle;

                Emit("routerclose");

                // Emit observer event.
                Observer.Emit("close");
            }
        }

        /// <summary>
        /// Pause the RtpObserver.
        /// </summary>
        public async Task PauseAsync()
        {
            _logger.LogDebug("PauseAsync() | RtpObserverId:{RtpObserverId}", Internal.RtpObserverId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("PauseAsync()");
                }

                await _pauseLock.WaitAsync();
                try
                {
                    var wasPaused = _paused;

                    // Build Request
                    var bufferBuilder = Channel.BufferPool.Get();

                    // Fire and forget
                    Channel.RequestAsync(bufferBuilder, Method.RTPOBSERVER_PAUSE,
                        null,
                        null,
                        Internal.RtpObserverId
                        ).ContinueWithOnFaultedHandleLog(_logger);

                    _paused = true;

                    // Emit observer event.
                    if(!wasPaused)
                    {
                        Observer.Emit("pause");
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "PauseAsync()");
                }
                finally
                {
                    _pauseLock.Set();
                }
            }
        }

        /// <summary>
        /// Resume the RtpObserver.
        /// </summary>
        public async Task ResumeAsync()
        {
            _logger.LogDebug("ResumeAsync() | RtpObserverId:{RtpObserverId}", Internal.RtpObserverId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("ResumeAsync()");
                }

                await _pauseLock.WaitAsync();
                try
                {
                    var wasPaused = _paused;

                    // Build Request
                    var bufferBuilder = Channel.BufferPool.Get();

                    // Fire and forget
                    Channel.RequestAsync(bufferBuilder, Method.RTPOBSERVER_RESUME,
                        null,
                        null,
                        Internal.RtpObserverId
                        ).ContinueWithOnFaultedHandleLog(_logger);

                    _paused = false;

                    // Emit observer event.
                    if(wasPaused)
                    {
                        Observer.Emit("resume");
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "ResumeAsync()");
                }
                finally
                {
                    _pauseLock.Set();
                }
            }
        }

        /// <summary>
        /// Add a Producer to the RtpObserver.
        /// </summary>
        public async Task AddProducerAsync(RtpObserverAddRemoveProducerOptions rtpObserverAddRemoveProducerOptions)
        {
            _logger.LogDebug("AddProducerAsync() | RtpObserverId:{RtpObserverId} ProducerId:{ProducerId}", Internal.RtpObserverId, rtpObserverAddRemoveProducerOptions.ProducerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("RepObserver closed");
                }

                var producer = GetProducerById(rtpObserverAddRemoveProducerOptions.ProducerId);
                if(producer == null)
                {
                    return;
                }

                // Build Request
                var bufferBuilder = Channel.BufferPool.Get();

                var addProducerRequest = new FBS.RtpObserver.AddProducerRequestT
                {
                    ProducerId = rtpObserverAddRemoveProducerOptions.ProducerId
                };

                var addProducerRequestOffset = FBS.RtpObserver.AddProducerRequest.Pack(bufferBuilder, addProducerRequest);

                // Fire and forget
                Channel.RequestAsync(bufferBuilder, Method.RTPOBSERVER_ADD_PRODUCER,
                    FBS.Request.Body.RtpObserver_AddProducerRequest,
                    addProducerRequestOffset.Value,
                    Internal.RtpObserverId
                    ).ContinueWithOnFaultedHandleLog(_logger);

                // Emit observer event.
                Observer.Emit("addproducer", producer);
            }
        }

        /// <summary>
        /// Remove a Producer from the RtpObserver.
        /// </summary>
        public async Task RemoveProducerAsync(RtpObserverAddRemoveProducerOptions rtpObserverAddRemoveProducerOptions)
        {
            _logger.LogDebug("RemoveProducerAsync() | RtpObserverId:{RtpObserverId}", Internal.RtpObserverId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("RepObserver closed");
                }

                var producer = GetProducerById(rtpObserverAddRemoveProducerOptions.ProducerId);
                if(producer == null)
                {
                    return;
                }

                // Build Request
                var bufferBuilder = Channel.BufferPool.Get();

                var removeProducerRequest = new FBS.RtpObserver.RemoveProducerRequestT
                {
                    ProducerId = rtpObserverAddRemoveProducerOptions.ProducerId
                };

                var removeProducerRequestOffset = FBS.RtpObserver.RemoveProducerRequest.Pack(bufferBuilder, removeProducerRequest);

                // Fire and forget
                Channel.RequestAsync(bufferBuilder, Method.RTPOBSERVER_REMOVE_PRODUCER,
                    FBS.Request.Body.RtpObserver_RemoveProducerRequest,
                    removeProducerRequestOffset.Value,
                    Internal.RtpObserverId
                    ).ContinueWithOnFaultedHandleLog(_logger);

                // Emit observer event.
                Observer.Emit("removeproducer", producer);
            }
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            Channel.OnNotification += OnNotificationHandle;
        }

        protected virtual void OnNotificationHandle(string handlerId, Event @event, Notification notification) { }

        #endregion Event Handlers
    }
}
