using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

namespace Tubumu.Mediasoup
{
    public class RtpObserverInternalData
    {
        /// <summary>
        /// Router id.
        /// </summary>
        public string RouterId { get; }

        /// <summary>
        /// RtpObserver id.
        /// </summary>
        public string RtpObserverId { get; }

        public RtpObserverInternalData(string routerId, string rtpObserverId)
        {
            RouterId = routerId;
            RtpObserverId = rtpObserverId;
        }
    }

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
        public RtpObserverInternalData Internal { get; private set; }

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
        public Dictionary<string, object>? AppData { get; private set; }

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
        /// <param name="rtpObserverInternalData"></param>
        /// <param name="channel"></param>
        /// <param name="payloadChannel"></param>
        /// <param name="appData"></param>
        /// <param name="getProducerById"></param>
        protected RtpObserver(ILoggerFactory loggerFactory,
            RtpObserverInternalData rtpObserverInternalData,
            IChannel channel,
            IPayloadChannel payloadChannel,
            Dictionary<string, object>? appData,
            Func<string, Task<Producer?>> getProducerById
            )
        {
            _logger = loggerFactory.CreateLogger<RtpObserver>();

            // Internal
            Internal = rtpObserverInternalData;

            Channel = channel;
            PayloadChannel = payloadChannel;
            AppData = appData;
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

                // Fire and forget.
                Channel.RequestAsync(MethodId.RTP_OBSERVER_CLOSE, Internal).ContinueWithOnFaultedHandleLog(_logger);

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

                    await Channel.RequestAsync(MethodId.RTP_OBSERVER_PAUSE, Internal);

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

                    await Channel.RequestAsync(MethodId.RTP_OBSERVER_RESUME, Internal);

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
                    throw new InvalidStateException("AddProducerAsync()");
                }

                var producer = GetProducerById(rtpObserverAddRemoveProducerOptions.ProducerId);
                if (producer == null)
                {
                    return;
                }

                var reqData = new { rtpObserverAddRemoveProducerOptions.ProducerId };
                await Channel.RequestAsync(MethodId.RTP_OBSERVER_ADD_PRODUCER, Internal, reqData);

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
                    throw new InvalidStateException("AddProducerAsync()");
                }

                var producer = GetProducerById(rtpObserverAddRemoveProducerOptions.ProducerId);
                if (producer == null)
                {
                    return;
                }

                var reqData = new { rtpObserverAddRemoveProducerOptions.ProducerId };
                await Channel.RequestAsync(MethodId.RTP_OBSERVER_REMOVE_PRODUCER, Internal, reqData);

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
