using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// PayloadChannel instance.
        /// </summary>
        protected readonly IPayloadChannel PayloadChannel;

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
        /// <param name="@internal"></param>
        /// <param name="channel"></param>
        /// <param name="payloadChannel"></param>
        /// <param name="appData"></param>
        /// <param name="getProducerById"></param>
        protected RtpObserver(ILoggerFactory loggerFactory,
            RtpObserverInternal @internal,
            IChannel channel,
            IPayloadChannel payloadChannel,
            Dictionary<string, object>? appData,
            Func<string, Task<Producer?>> getProducerById
            )
        {
            _logger = loggerFactory.CreateLogger<RtpObserver>();

            Internal = @internal;
            Channel = channel;
            PayloadChannel = payloadChannel;
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
            _logger.LogDebug($"Close() | RtpObserver:{Internal.RtpObserverId}");

            using (await _closeLock.WriteLockAsync())
            {
                if (_closed)
                {
                    return;
                }

                _closed = true;

                // Remove notification subscriptions.
                Channel.MessageEvent -= OnChannelMessage;
                //PayloadChannel.MessageEvent -= OnPayloadChannelMessage;

                var reqData = new { RtpObserverId = Internal.RtpObserverId };

                // Fire and forget
                Channel.RequestAsync(MethodId.ROUTER_CLOSE_RTP_OBSERVER, Internal.RouterId, reqData).ContinueWithOnFaultedHandleLog(_logger);

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
            _logger.LogDebug($"RouterClosed() | RtpObserver:{Internal.RtpObserverId}");

            using (await _closeLock.WriteLockAsync())
            {
                if (_closed)
                {
                    return;
                }

                _closed = true;

                // Remove notification subscriptions.
                Channel.MessageEvent -= OnChannelMessage;
                //PayloadChannel.MessageEvent -= OnPayloadChannelMessage;

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
            _logger.LogDebug($"PauseAsync() | RtpObserver:{Internal.RtpObserverId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("PauseAsync()");
                }

                await _pauseLock.WaitAsync();
                try
                {
                    var wasPaused = _paused;

                    // Fire and forget
                    Channel.RequestAsync(MethodId.RTP_OBSERVER_PAUSE, Internal.RtpObserverId).ContinueWithOnFaultedHandleLog(_logger);

                    _paused = true;

                    // Emit observer event.
                    if (!wasPaused)
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
            _logger.LogDebug($"ResumeAsync() | RtpObserver:{Internal.RtpObserverId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("ResumeAsync()");
                }

                await _pauseLock.WaitAsync();
                try
                {
                    var wasPaused = _paused;

                    // Fire and forget
                    Channel.RequestAsync(MethodId.RTP_OBSERVER_RESUME, Internal.RtpObserverId).ContinueWithOnFaultedHandleLog(_logger);

                    _paused = false;

                    // Emit observer event.
                    if (wasPaused)
                    {
                        Observer.Emit("resume");
                    }
                }
                catch (Exception ex)
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
            _logger.LogDebug($"AddProducerAsync() | RtpObserver:{Internal.RtpObserverId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("RepObserver closed");
                }

                var producer = GetProducerById(rtpObserverAddRemoveProducerOptions.ProducerId);
                if (producer == null)
                {
                    return;
                }

                var reqData = new { rtpObserverAddRemoveProducerOptions.ProducerId };
                // Fire and forget
                Channel.RequestAsync(MethodId.RTP_OBSERVER_ADD_PRODUCER, Internal.RtpObserverId, reqData).ContinueWithOnFaultedHandleLog(_logger);

                // Emit observer event.
                Observer.Emit("addproducer", producer);
            }
        }

        /// <summary>
        /// Remove a Producer from the RtpObserver.
        /// </summary>
        public async Task RemoveProducerAsync(RtpObserverAddRemoveProducerOptions rtpObserverAddRemoveProducerOptions)
        {
            _logger.LogDebug($"RemoveProducerAsync() | RtpObserver:{Internal.RtpObserverId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("RepObserver closed");
                }

                var producer = GetProducerById(rtpObserverAddRemoveProducerOptions.ProducerId);
                if (producer == null)
                {
                    return;
                }

                var reqData = new { rtpObserverAddRemoveProducerOptions.ProducerId };
                // Fire and forget
                Channel.RequestAsync(MethodId.RTP_OBSERVER_REMOVE_PRODUCER, Internal.RtpObserverId, reqData).ContinueWithOnFaultedHandleLog(_logger);

                // Emit observer event.
                Observer.Emit("removeproducer", producer);
            }
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            Channel.MessageEvent += OnChannelMessage;
        }

        protected abstract void OnChannelMessage(string targetId, string @event, string? data);

        #endregion Event Handlers
    }
}
