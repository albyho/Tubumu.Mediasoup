using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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

        // TODO: (alby) _closed 的使用及线程安全。
        /// <summary>
        /// Whether the Producer is closed.
        /// </summary>
        private bool _closed;

        // TODO: (alby) _paused 的使用及线程安全。
        /// <summary>
        /// Paused flag.
        /// </summary>
        private bool _paused;

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
        protected readonly Func<string, Producer?> GetProducerById;

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
            Func<string, Producer?> getProducerById
            )
        {
            _logger = loggerFactory.CreateLogger<RtpObserver>();

            // Internal
            Internal = rtpObserverInternalData;

            Channel = channel;
            PayloadChannel = payloadChannel;
            AppData = appData;
            GetProducerById = getProducerById;
            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the RtpObserver.
        /// </summary>
        public void Close()
        {
            _logger.LogDebug($"Close() | RtpObserver:{Internal.RtpObserverId}");

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

        /// <summary>
        /// Router was closed.
        /// </summary>
        public void RouterClosed()
        {
            _logger.LogDebug($"RouterClosed() | RtpObserver:{Internal.RtpObserverId}");

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

        /// <summary>
        /// Pause the RtpObserver.
        /// </summary>
        public async Task PauseAsync()
        {
            _logger.LogDebug($"PauseAsync() | RtpObserver:{Internal.RtpObserverId}");

            var wasPaused = _paused;

            await Channel.RequestAsync(MethodId.RTP_OBSERVER_PAUSE, Internal);

            _paused = true;

            // Emit observer event.
            if (!wasPaused)
            {
                Observer.Emit("pause");
            }
        }

        /// <summary>
        /// Resume the RtpObserver.
        /// </summary>
        public async Task ResumeAsync()
        {
            _logger.LogDebug($"ResumeAsync() | RtpObserver:{Internal.RtpObserverId}");

            var wasPaused = _paused;

            await Channel.RequestAsync(MethodId.RTP_OBSERVER_RESUME, Internal);

            _paused = false;

            // Emit observer event.
            if (wasPaused)
            {
                Observer.Emit("resume");
            }
        }

        /// <summary>
        /// Add a Producer to the RtpObserver.
        /// </summary>
        public async Task AddProducerAsync(RtpObserverAddRemoveProducerOptions rtpObserverAddRemoveProducerOptions)
        {
            _logger.LogDebug($"AddProducerAsync() | RtpObserver:{Internal.RtpObserverId}");

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

        /// <summary>
        /// Remove a Producer from the RtpObserver.
        /// </summary>
        public async Task RemoveProducerAsync(RtpObserverAddRemoveProducerOptions rtpObserverAddRemoveProducerOptions)
        {
            _logger.LogDebug($"RemoveProducerAsync() | RtpObserver:{Internal.RtpObserverId}");

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

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            Channel.MessageEvent += OnChannelMessage;
        }

        protected abstract void OnChannelMessage(string targetId, string @event, string? data);

        #endregion Event Handlers
    }
}
