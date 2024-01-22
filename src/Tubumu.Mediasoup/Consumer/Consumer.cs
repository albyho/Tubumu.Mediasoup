using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FBS.Consumer;
using FBS.Notification;
using FBS.Request;
using FBS.RtpStream;
using Google.FlatBuffers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

namespace Tubumu.Mediasoup
{
    public class Consumer : EventEmitter
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<Consumer> _logger;

        /// <summary>
        /// Whether the Consumer is closed.
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
        private readonly ConsumerInternal _internal;

        /// <summary>
        /// Consumer id.
        /// </summary>
        public string ConsumerId => _internal.ConsumerId;

        /// <summary>
        /// Consumer data.
        /// </summary>
        public ConsumerData Data { get; set; }

        /// <summary>
        /// Producer id.
        /// </summary>
        public string ProducerId => Data.ProducerId;

        /// <summary>
        /// Channel instance.
        /// </summary>
        private readonly IChannel _channel;

        /// <summary>
        /// App custom data.
        /// </summary>
        public Dictionary<string, object> AppData { get; }

        /// <summary>
        /// [扩展]Source.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Whether the associate Producer is paused.
        /// </summary>
        public bool ProducerPaused { get; private set; }

        /// <summary>
        /// Current priority.
        /// </summary>
        public int Priority { get; private set; } = 1;

        /// <summary>
        /// Current score.
        /// </summary>
        public ConsumerScoreT? Score;

        /// <summary>
        /// Preferred layers.
        /// </summary>
        public ConsumerLayersT? PreferredLayers { get; private set; }

        /// <summary>
        /// Curent layers.
        /// </summary>
        public ConsumerLayersT? CurrentLayers { get; private set; }

        /// <summary>
        /// Observer instance.
        /// </summary>
        public EventEmitter Observer { get; } = new EventEmitter();

        /// <summary>
        /// <para>Events:</para>
        /// <para>@emits transportclose</para>
        /// <para>@emits producerclose</para>
        /// <para>@emits producerpause</para>
        /// <para>@emits producerresume</para>
        /// <para>@emits score - (score: ConsumerScore)</para>
        /// <para>@emits layerschange - (layers: ConsumerLayers | undefined)</para>
        /// <para>@emits trace - (trace: ConsumerTraceEventData)</para>
        /// <para>@emits @close</para>
        /// <para>@emits @producerclose</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// <para>@emits pause</para>
        /// <para>@emits resume</para>
        /// <para>@emits score - (score: ConsumerScore)</para>
        /// <para>@emits layerschange - (layers: ConsumerLayers | undefined)</para>
        /// <para>@emits rtp - (packet: Buffer)</para>
        /// <para>@emits trace - (trace: ConsumerTraceEventData)</para>
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="internal_"></param>
        /// <param name="data"></param>
        /// <param name="channel"></param>
        /// <param name="appData"></param>
        /// <param name="paused"></param>
        /// <param name="producerPaused"></param>
        /// <param name="score"></param>
        /// <param name="preferredLayers"></param>
        public Consumer(
            ILoggerFactory loggerFactory,
            ConsumerInternal internal_,
            ConsumerData data,
            IChannel channel,
            Dictionary<string, object>? appData,
            bool paused,
            bool producerPaused,
            FBS.Consumer.ConsumerScoreT? score,
            ConsumerLayersT? preferredLayers
        )
        {
            _logger = loggerFactory.CreateLogger<Consumer>();

            _internal = internal_;
            Data = data;
            _channel = channel;
            AppData = appData ?? new Dictionary<string, object>();
            _paused = paused;
            ProducerPaused = producerPaused;
            Score = score;
            PreferredLayers = preferredLayers;
            _pauseLock.Set();

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the Producer.
        /// </summary>
        public async Task CloseAsync()
        {
            _logger.LogDebug("CloseAsync() | Consumer:{ConsumerId}", ConsumerId);

            using(await _closeLock.WriteLockAsync())
            {
                if(_closed)
                {
                    return;
                }

                _closed = true;

                // Remove notification subscriptions.
                _channel.OnNotification -= OnNotificationHandle;

                // Build Request
                var bufferBuilder = new FlatBufferBuilder(1024);

                var requestOffset = FBS.Transport.CloseConsumerRequest.Pack(bufferBuilder, new FBS.Transport.CloseConsumerRequestT
                {
                    ConsumerId = _internal.ConsumerId
                });

                // Fire and forget
                _channel.RequestAsync(
                    bufferBuilder,
                    Method.TRANSPORT_CLOSE_CONSUMER,
                    FBS.Request.Body.Transport_CloseConsumerRequest,
                    requestOffset.Value,
                    _internal.TransportId
                    )
                    .ContinueWithOnFaultedHandleLog(_logger);

                Emit("@close");

                // Emit observer event.
                Observer.Emit("close");
            }
        }

        /// <summary>
        /// Transport was closed.
        /// </summary>
        public async Task TransportClosedAsync()
        {
            _logger.LogDebug("TransportClosed() | Consumer:{ConsumerId}", ConsumerId);

            using(await _closeLock.WriteLockAsync())
            {
                if(_closed)
                {
                    return;
                }

                _closed = true;

                // Remove notification subscriptions.
                _channel.OnNotification -= OnNotificationHandle;

                Emit("transportclose");

                // Emit observer event.
                Observer.Emit("close");
            }
        }

        /// <summary>
        /// Dump DataProducer.
        /// </summary>
        public async Task<DumpResponseT> DumpAsync()
        {
            _logger.LogDebug("DumpAsync() | Consumer:{ConsumerId}", ConsumerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                var bufferBuilder = new FlatBufferBuilder(1024);
                var response = await _channel.RequestAsync(bufferBuilder, Method.CONSUMER_DUMP, null, null, _internal.ConsumerId);
                var data = response.Value.BodyAsConsumer_DumpResponse().UnPack();
                return data;
            }
        }

        /// <summary>
        /// Get DataProducer stats.
        /// </summary>
        public async Task<List<StatsT>> GetStatsAsync()
        {
            _logger.LogDebug("GetStatsAsync() | Consumer:{ConsumerId}", ConsumerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                var bufferBuilder = new FlatBufferBuilder(1024);
                var response = await _channel.RequestAsync(bufferBuilder, Method.CONSUMER_GET_STATS, null, null, _internal.ConsumerId);
                var stats = response.Value.BodyAsConsumer_GetStatsResponse().UnPack().Stats;
                return stats;
            }
        }

        /// <summary>
        /// Pause the Consumer.
        /// </summary>
        public async Task PauseAsync()
        {
            _logger.LogDebug("PauseAsync() | Consumer:{ConsumerId}", ConsumerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                await _pauseLock.WaitAsync();
                try
                {
                    var wasPaused = _paused || ProducerPaused;

                    var bufferBuilder = new FlatBufferBuilder(1024);

                    // Fire and forget
                    _channel.RequestAsync(bufferBuilder, Method.CONSUMER_PAUSE, null, null, _internal.ConsumerId)
                        .ContinueWithOnFaultedHandleLog(_logger);

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
        /// Resume the Consumer.
        /// </summary>
        public async Task ResumeAsync()
        {
            _logger.LogDebug("ResumeAsync() | Consumer:{ConsumerId}", ConsumerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                await _pauseLock.WaitAsync();
                try
                {
                    var wasPaused = _paused || ProducerPaused;

                    var bufferBuilder = new FlatBufferBuilder(1024);

                    // Fire and forget
                    _channel.RequestAsync(bufferBuilder, Method.CONSUMER_RESUME, null, null, _internal.ConsumerId)
                        .ContinueWithOnFaultedHandleLog(_logger);

                    _paused = false;

                    // Emit observer event.
                    if(wasPaused && !ProducerPaused)
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
        /// Set preferred video layers.
        /// </summary>
        public async Task SetPreferredLayersAsync(SetPreferredLayersRequestT setPreferredLayersRequest)
        {
            _logger.LogDebug("SetPreferredLayersAsync() | Consumer:{ConsumerId}", ConsumerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                // Build Request
                var bufferBuilder = new FlatBufferBuilder(1024);

                var setPreferredLayersRequestOffset = SetPreferredLayersRequest.Pack(bufferBuilder, setPreferredLayersRequest);

                var response = await _channel.RequestAsync(
                    bufferBuilder,
                    Method.CONSUMER_SET_PREFERRED_LAYERS,
                    FBS.Request.Body.Consumer_SetPreferredLayersRequest,
                    setPreferredLayersRequestOffset.Value,
                    _internal.ConsumerId);
                var preferredLayers = response.Value.BodyAsConsumer_SetPreferredLayersResponse().UnPack().PreferredLayers;

                PreferredLayers = preferredLayers;
            }
        }

        /// <summary>
        /// Set priority.
        /// </summary>
        public async Task SetPriorityAsync(SetPriorityRequestT setPriorityRequest)
        {
            _logger.LogDebug("SetPriorityAsync() | Consumer:{ConsumerId}", ConsumerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                var bufferBuilder = new FlatBufferBuilder(1024);

                var setPriorityRequestOffset = SetPriorityRequest.Pack(bufferBuilder, setPriorityRequest);

                var response = await _channel.RequestAsync(
                    bufferBuilder,
                    Method.CONSUMER_SET_PRIORITY,
                    FBS.Request.Body.Consumer_SetPriorityRequest,
                    setPriorityRequestOffset.Value,
                    _internal.ConsumerId);

                var priorityResponse = response.Value.BodyAsConsumer_SetPriorityResponse().UnPack().Priority;

                Priority = priorityResponse;
            }
        }

        /// <summary>
        /// Unset priority.
        /// </summary>
        public Task UnsetPriorityAsync()
        {
            _logger.LogDebug("UnsetPriorityAsync() | Consumer:{ConsumerId}", ConsumerId);

            return SetPriorityAsync(new SetPriorityRequestT
            {
                Priority = 1,
            });
        }

        /// <summary>
        /// Request a key frame to the Producer.
        /// </summary>
        public async Task RequestKeyFrameAsync()
        {
            _logger.LogDebug("RequestKeyFrameAsync() | Consumer:{ConsumerId}", ConsumerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                // Build Request
                var bufferBuilder = new FlatBufferBuilder(1024);

                await _channel.RequestAsync(bufferBuilder, Method.CONSUMER_REQUEST_KEY_FRAME,
                    null,
                    null,
                    _internal.ConsumerId);
            }
        }

        /// <summary>
        /// Enable 'trace' event.
        /// </summary>
        public async Task EnableTraceEventAsync(List<TraceEventType> types)
        {
            _logger.LogDebug("EnableTraceEventAsync() | Consumer:{ConsumerId}", ConsumerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                // Build Request
                var bufferBuilder = new FlatBufferBuilder(1024);

                var request = new EnableTraceEventRequestT
                {
                    Events = types ?? new List<TraceEventType>(0)
                };

                var requestOffset = FBS.Consumer.EnableTraceEventRequest.Pack(bufferBuilder, request);

                // Fire and forget
                _channel.RequestAsync(
                    bufferBuilder,
                    Method.CONSUMER_ENABLE_TRACE_EVENT,
                    FBS.Request.Body.Consumer_EnableTraceEventRequest,
                    requestOffset.Value,
                     _internal.ConsumerId)
                    .ContinueWithOnFaultedHandleLog(_logger);
            }
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            _channel.OnNotification += OnNotificationHandle;
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void OnNotificationHandle(string handlerId, Event @event, Notification notification)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            if(handlerId != ConsumerId)
            {
                return;
            }

            switch(@event)
            {
                case Event.CONSUMER_PRODUCER_CLOSE:
                    {
                        using(await _closeLock.WriteLockAsync())
                        {
                            if(_closed)
                            {
                                break;
                            }

                            _closed = true;

                            // Remove notification subscriptions.
                            _channel.OnNotification -= OnNotificationHandle;

                            Emit("@producerclose");
                            Emit("producerclose");

                            // Emit observer event.
                            Observer.Emit("close");
                        }

                        break;
                    }
                case Event.CONSUMER_PRODUCER_PAUSE:
                    {
                        if(ProducerPaused)
                        {
                            break;
                        }

                        var wasPaused = _paused || ProducerPaused;

                        ProducerPaused = true;

                        Emit("producerpause");

                        // Emit observer event.
                        if(!wasPaused)
                        {
                            Observer.Emit("pause");
                        }

                        break;
                    }
                case Event.CONSUMER_PRODUCER_RESUME:
                    {
                        if(!ProducerPaused)
                        {
                            break;
                        }

                        var wasPaused = _paused || ProducerPaused;

                        ProducerPaused = false;

                        Emit("producerresume");

                        // Emit observer event.
                        if(wasPaused && !_paused)
                        {
                            Observer.Emit("resume");
                        }

                        break;
                    }
                case Event.CONSUMER_SCORE:
                    {
                        var scoreNotification = notification.BodyAsConsumer_ScoreNotification();
                        var score = scoreNotification.Score!.Value.UnPack();
                        Score = score;

                        Emit("score", Score);

                        // Emit observer event.
                        Observer.Emit("score", Score);

                        break;
                    }
                case Event.CONSUMER_LAYERS_CHANGE:
                    {
                        var layersChangeNotification = notification.BodyAsConsumer_LayersChangeNotification();
                        var currentLayers = layersChangeNotification.Layers!.Value.UnPack();
                        CurrentLayers = currentLayers;

                        Emit("layerschange", CurrentLayers);

                        // Emit observer event.
                        Observer.Emit("layersChange", CurrentLayers);

                        break;
                    }
                case Event.CONSUMER_TRACE:
                    {
                        var traceNotification = notification.BodyAsConsumer_TraceNotification();
                        var trace = traceNotification.UnPack();

                        Emit("trace", trace);

                        // Emit observer event.
                        Observer.Emit("trace", trace);

                        break;
                    }
                default:
                    {
                        _logger.LogError("OnNotificationHandle() | Ignoring unknown event{@event}", @event);
                        break;
                    }
            }
        }
    }

    #endregion Event Handlers
}
