using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using Tubumu.Mediasoup.Extensions;
using Tubumu.Utils.Extensions;
using ObjectExtensions = Tubumu.Utils.Extensions.Object.ObjectExtensions;

namespace Tubumu.Mediasoup
{
    public class ConsumerInternalData
    {
        /// <summary>
        /// Router id.
        /// </summary>
        public string RouterId { get; }

        /// <summary>
        /// Transport id.
        /// </summary>
        public string TransportId { get; }

        /// <summary>
        /// Associated Producer id.
        /// </summary>
        public string ProducerId { get; }

        /// <summary>
        /// Consumer id.
        /// </summary>
        public string ConsumerId { get; }

        public ConsumerInternalData(string routerId, string transportId, string producerId, string consumerId)
        {
            RouterId = routerId;
            TransportId = transportId;
            ProducerId = producerId;
            ConsumerId = consumerId;
        }
    }

    public class Consumer : EventEmitter
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<Consumer> _logger;

        // TODO: (alby) _closed 的使用及线程安全。
        /// <summary>
        /// Whether the Consumer is closed.
        /// </summary>
        private bool _closed;

        private readonly AsyncReaderWriterLock _closeLock = new AsyncReaderWriterLock();

        // TODO: (alby) _paused 的使用及线程安全。
        /// <summary>
        /// Paused flag.
        /// </summary>
        private bool _paused;

        /// <summary>
        /// Internal data.
        /// </summary>
        private readonly ConsumerInternalData _internal;

        /// <summary>
        /// Consumer id.
        /// </summary>
        public string ConsumerId => _internal.ConsumerId;

        /// <summary>
        /// Consumer id.
        /// </summary>
        public string ProducerId => _internal.ProducerId;

        #region Consumer data.

        /// <summary>
        /// Media kind.
        /// </summary>
        public MediaKind Kind { get; }

        /// <summary>
        /// RTP parameters.
        /// </summary>
        public RtpParameters RtpParameters { get; }

        /// <summary>
        /// Consumer type.
        /// </summary>
        public ConsumerType Type { get; }

        #endregion Consumer data.

        /// <summary>
        /// Channel instance.
        /// </summary>
        private readonly Channel _channel;

        /// <summary>
        /// PayloadChannel instance.
        /// </summary>
        private readonly PayloadChannel _payloadChannel;

        /// <summary>
        /// App custom data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; private set; }

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
        public ConsumerScore? Score;

        /// <summary>
        /// Preferred layers.
        /// </summary>
        public ConsumerLayers? PreferredLayers { get; private set; }

        /// <summary>
        /// Curent layers.
        /// </summary>
        public ConsumerLayers? CurrentLayers { get; private set; }

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
        /// <param name="consumerInternalData"></param>
        /// <param name="kind"></param>
        /// <param name="rtpParameters"></param>
        /// <param name="type"></param>
        /// <param name="channel"></param>
        /// <param name="appData"></param>
        /// <param name="paused"></param>
        /// <param name="producerPaused"></param>
        /// <param name="score"></param>
        /// <param name="preferredLayers"></param>
        public Consumer(ILoggerFactory loggerFactory,
            ConsumerInternalData consumerInternalData,
            MediaKind kind,
            RtpParameters rtpParameters,
            ConsumerType type,
            Channel channel,
            PayloadChannel payloadChannel,
            Dictionary<string, object>? appData,
            bool paused,
            bool producerPaused,
            ConsumerScore? score,
            ConsumerLayers? preferredLayers
            )
        {
            _logger = loggerFactory.CreateLogger<Consumer>();

            // Internal
            _internal = consumerInternalData;

            // Data
            Kind = kind;
            RtpParameters = rtpParameters;
            Type = type;

            _channel = channel;
            _payloadChannel = payloadChannel;
            AppData = appData;
            _paused = paused;
            ProducerPaused = producerPaused;
            Score = score;
            PreferredLayers = preferredLayers;

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the Producer.
        /// </summary>
        public async Task CloseAsync()
        {
            if (_closed)
            {
                return;
            }

            using (await _closeLock.WriteLockAsync())
            {
                if (_closed)
                {
                    return;
                }

                _logger.LogDebug($"Close() | Consumer:{ConsumerId}");

                _closed = true;

                // Remove notification subscriptions.
                _channel.MessageEvent -= OnChannelMessage;
                _payloadChannel.MessageEvent -= OnPayloadChannelMessage;

                // Fire and forget
                _channel.RequestAsync(MethodId.CONSUMER_CLOSE, _internal).ContinueWithOnFaultedHandleLog(_logger);

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
            if (_closed)
            {
                return;
            }

            using (await _closeLock.WriteLockAsync())
            {
                if (_closed)
                {
                    return;
                }

                _logger.LogDebug($"TransportClosed() | Consumer:{ConsumerId}");

                _closed = true;

                // Remove notification subscriptions.
                _channel.MessageEvent -= OnChannelMessage;
                _payloadChannel.MessageEvent -= OnPayloadChannelMessage;

                Emit("transportclose");

                // Emit observer event.
                Observer.Emit("close");
            }
        }

        /// <summary>
        /// Dump DataProducer.
        /// </summary>
        public Task<string?> DumpAsync()
        {
            _logger.LogDebug($"DumpAsync() | Consumer:{ConsumerId}");

            return _channel.RequestAsync(MethodId.CONSUMER_DUMP, _internal);
        }

        /// <summary>
        /// Get DataProducer stats.
        /// </summary>
        public Task<string?> GetStatsAsync()
        {
            _logger.LogDebug($"GetStatsAsync() | Consumer:{ConsumerId}");

            return _channel.RequestAsync(MethodId.CONSUMER_GET_STATS, _internal);
        }

        /// <summary>
        /// Pause the Consumer.
        /// </summary>
        public async Task PauseAsync()
        {
            _logger.LogDebug($"PauseAsync() | Consumer:{ConsumerId}");

            var wasPaused = _paused || ProducerPaused;

            await _channel.RequestAsync(MethodId.CONSUMER_PAUSE, _internal);

            _paused = true;

            // Emit observer event.
            if (!wasPaused)
            {
                Observer.Emit("pause");
            }
        }

        /// <summary>
        /// Resume the Consumer.
        /// </summary>
        public async Task ResumeAsync()
        {
            _logger.LogDebug($"ResumeAsync() | Consumer:{ConsumerId}");

            var wasPaused = _paused || ProducerPaused;

            await _channel.RequestAsync(MethodId.CONSUMER_RESUME, _internal);

            _paused = false;

            // Emit observer event.
            if (wasPaused && !ProducerPaused)
            {
                Observer.Emit("resume");
            }
        }

        /// <summary>
        /// Set preferred video layers.
        /// </summary>
        public async Task SetPreferredLayersAsync(ConsumerLayers consumerLayers)
        {
            _logger.LogDebug($"SetPreferredLayersAsync() | Consumer:{ConsumerId}");

            var reqData = consumerLayers;
            var resData = await _channel.RequestAsync(MethodId.CONSUMER_SET_PREFERRED_LAYERS, _internal, reqData);
            var responseData = JsonSerializer.Deserialize<ConsumerSetPreferredLayersResponseData>(resData!, ObjectExtensions.DefaultJsonSerializerOptions);
            PreferredLayers = responseData;
        }

        /// <summary>
        /// Set priority.
        /// </summary>
        public async Task SetPriorityAsync(int priority)
        {
            _logger.LogDebug($"SetPriorityAsync() | Consumer:{ConsumerId}");

            var reqData = new { Priority = priority };
            var resData = await _channel.RequestAsync(MethodId.CONSUMER_SET_PRIORITY, _internal, reqData);
            var responseData = JsonSerializer.Deserialize<ConsumerSetOrUnsetPriorityResponseData>(resData!, ObjectExtensions.DefaultJsonSerializerOptions);
            Priority = responseData!.Priority;
        }

        /// <summary>
        /// Unset priority.
        /// </summary>
        public async Task UnsetPriorityAsync()
        {
            _logger.LogDebug($"UnsetPriorityAsync() | Consumer:{ConsumerId}");

            var reqData = new { Priority = 1 };
            var resData = await _channel.RequestAsync(MethodId.CONSUMER_SET_PRIORITY, _internal, reqData);
            var responseData = JsonSerializer.Deserialize<ConsumerSetOrUnsetPriorityResponseData>(resData!, ObjectExtensions.DefaultJsonSerializerOptions);
            Priority = responseData!.Priority;
        }

        /// <summary>
        /// Request a key frame to the Producer.
        /// </summary>
        public Task RequestKeyFrameAsync()
        {
            _logger.LogDebug($"RequestKeyFrameAsync() | Consumer:{ConsumerId}");

            return _channel.RequestAsync(MethodId.CONSUMER_REQUEST_KEY_FRAME, _internal);
        }

        /// <summary>
        /// Enable 'trace' event.
        /// </summary>
        public Task EnableTraceEventAsync(TraceEventType[] types)
        {
            _logger.LogDebug($"EnableTraceEventAsync() | Consumer:{ConsumerId}");

            var reqData = new
            {
                Types = types ?? Array.Empty<TraceEventType>()
            };
            return _channel.RequestAsync(MethodId.CONSUMER_ENABLE_TRACE_EVENT, _internal, reqData);
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            _channel.MessageEvent += OnChannelMessage;
            _payloadChannel.MessageEvent += OnPayloadChannelMessage;
        }

        private void OnChannelMessage(string targetId, string @event, string? data)
        {
            if (targetId != ConsumerId)
            {
                return;
            }

            switch (@event)
            {
                case "producerclose":
                    {
                        // TODO: (alby) _closed 的使用及线程安全。
                        if (_closed)
                        {
                            break;
                        }

                        _closed = true;

                        // Remove notification subscriptions.
                        _channel.MessageEvent -= OnChannelMessage;
                        _payloadChannel.MessageEvent -= OnPayloadChannelMessage;

                        Emit("@producerclose");
                        Emit("producerclose");

                        // Emit observer event.
                        Observer.Emit("close");

                        break;
                    }
                case "producerpause":
                    {
                        if (ProducerPaused)
                        {
                            break;
                        }

                        var wasPaused = _paused || ProducerPaused;

                        ProducerPaused = true;

                        Emit("producerpause");

                        // Emit observer event.
                        if (!wasPaused)
                        {
                            Observer.Emit("pause");
                        }

                        break;
                    }
                case "producerresume":
                    {
                        if (!ProducerPaused)
                        {
                            break;
                        }

                        var wasPaused = _paused || ProducerPaused;

                        ProducerPaused = false;

                        Emit("producerresume");

                        // Emit observer event.
                        if (wasPaused && !_paused)
                        {
                            Observer.Emit("resume");
                        }

                        break;
                    }
                case "score":
                    {
                        var score = JsonSerializer.Deserialize<ConsumerScore>(data!, ObjectExtensions.DefaultJsonSerializerOptions);
                        Score = score;

                        Emit("score", score);

                        // Emit observer event.
                        Observer.Emit("score", score);

                        break;
                    }
                case "layerschange":
                    {
                        var layers = !data.IsNullOrWhiteSpace() ? JsonSerializer.Deserialize<ConsumerLayers>(data!, ObjectExtensions.DefaultJsonSerializerOptions) : null;

                        CurrentLayers = layers;

                        Emit("layerschange", layers);

                        // Emit observer event.
                        Observer.Emit("layersChange", layers);

                        break;
                    }
                case "trace":
                    {
                        var trace = JsonSerializer.Deserialize<TransportTraceEventData>(data!, ObjectExtensions.DefaultJsonSerializerOptions);

                        Emit("trace", trace);

                        // Emit observer event.
                        Observer.Emit("trace", trace);

                        break;
                    }
                default:
                    {
                        _logger.LogError($"OnChannelMessage() | Ignoring unknown event{@event}");
                        break;
                    }
            }
        }

        private void OnPayloadChannelMessage(string targetId, string @event, NotifyData notifyData, ArraySegment<byte> payload)
        {
            if (targetId != ConsumerId)
            {
                return;
            }

            switch (@event)
            {
                case "rtp":
                    {
                        // TODO: (alby) _closed 的使用及线程安全。
                        if (_closed)
                            break;

                        Emit("rtp", payload);

                        break;
                    }
                default:
                    {
                        _logger.LogError($"OnPayloadChannelMessage() | Ignoring unknown event \"{@event}\"");
                        break;
                    }
            }
        }
    }

    #endregion Event Handlers
}
