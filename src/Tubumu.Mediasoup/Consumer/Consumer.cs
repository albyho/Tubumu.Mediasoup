using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
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
        /// PayloadChannel instance.
        /// </summary>
        private readonly IPayloadChannel _payloadChannel;

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
        /// <param name="@internal"></param>
        /// <param name="data"></param>
        /// <param name="channel"></param>
        /// <param name="appData"></param>
        /// <param name="paused"></param>
        /// <param name="producerPaused"></param>
        /// <param name="score"></param>
        /// <param name="preferredLayers"></param>
        public Consumer(ILoggerFactory loggerFactory,
            ConsumerInternal @internal,
            ConsumerData data,
            IChannel channel,
            IPayloadChannel payloadChannel,
            Dictionary<string, object>? appData,
            bool paused,
            bool producerPaused,
            ConsumerScore? score,
            ConsumerLayers? preferredLayers
            )
        {
            _logger = loggerFactory.CreateLogger<Consumer>();

            _internal = @internal;
            Data = data;
            _channel = channel;
            _payloadChannel = payloadChannel;
            AppData = appData;
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
            _logger.LogDebug($"CloseAsync() | Consumer:{ConsumerId}");

            using (await _closeLock.WriteLockAsync())
            {
                if (_closed)
                {
                    return;
                }

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
            _logger.LogDebug($"TransportClosed() | Consumer:{ConsumerId}");

            using (await _closeLock.WriteLockAsync())
            {
                if (_closed)
                {
                    return;
                }

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
        public async Task<string> DumpAsync()
        {
            _logger.LogDebug($"DumpAsync() | Consumer:{ConsumerId}");

            using (await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                return (await _channel.RequestAsync(MethodId.CONSUMER_DUMP, _internal))!;
            }
        }

        /// <summary>
        /// Get DataProducer stats.
        /// </summary>
        public async Task<string> GetStatsAsync()
        {
            _logger.LogDebug($"GetStatsAsync() | Consumer:{ConsumerId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                return (await _channel.RequestAsync(MethodId.CONSUMER_GET_STATS, _internal))!;
            }
        }

        /// <summary>
        /// Pause the Consumer.
        /// </summary>
        public async Task PauseAsync()
        {
            _logger.LogDebug($"PauseAsync() | Consumer:{ConsumerId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                await _pauseLock.WaitAsync();
                try
                {
                    var wasPaused = _paused || ProducerPaused;

                    // Fire and forget
                    _channel.RequestAsync(MethodId.CONSUMER_PAUSE, _internal).ContinueWithOnFaultedHandleLog(_logger);

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
        /// Resume the Consumer.
        /// </summary>
        public async Task ResumeAsync()
        {
            _logger.LogDebug($"ResumeAsync() | Consumer:{ConsumerId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                await _pauseLock.WaitAsync();
                try
                {
                    var wasPaused = _paused || ProducerPaused;

                    // Fire and forget
                    _channel.RequestAsync(MethodId.CONSUMER_RESUME, _internal).ContinueWithOnFaultedHandleLog(_logger);

                    _paused = false;

                    // Emit observer event.
                    if (wasPaused && !ProducerPaused)
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
        /// Set preferred video layers.
        /// </summary>
        public async Task SetPreferredLayersAsync(ConsumerLayers consumerLayers)
        {
            _logger.LogDebug($"SetPreferredLayersAsync() | Consumer:{ConsumerId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                var reqData = consumerLayers;
                var resData = await _channel.RequestAsync(MethodId.CONSUMER_SET_PREFERRED_LAYERS, _internal, reqData);
                var responseData = JsonSerializer.Deserialize<ConsumerSetPreferredLayersResponseData>(resData!, ObjectExtensions.DefaultJsonSerializerOptions);
                PreferredLayers = responseData;
            }
        }

        /// <summary>
        /// Set priority.
        /// </summary>
        public async Task SetPriorityAsync(int priority)
        {
            _logger.LogDebug($"SetPriorityAsync() | Consumer:{ConsumerId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                var reqData = new { Priority = priority };
                var resData = await _channel.RequestAsync(MethodId.CONSUMER_SET_PRIORITY, _internal, reqData);
                var responseData = JsonSerializer.Deserialize<ConsumerSetOrUnsetPriorityResponseData>(resData!, ObjectExtensions.DefaultJsonSerializerOptions);
                Priority = responseData!.Priority;
            }
        }

        /// <summary>
        /// Unset priority.
        /// </summary>
        public async Task UnsetPriorityAsync()
        {
            _logger.LogDebug($"UnsetPriorityAsync() | Consumer:{ConsumerId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                var reqData = new { Priority = 1 };
                var resData = await _channel.RequestAsync(MethodId.CONSUMER_SET_PRIORITY, _internal, reqData);
                var responseData = JsonSerializer.Deserialize<ConsumerSetOrUnsetPriorityResponseData>(resData!, ObjectExtensions.DefaultJsonSerializerOptions);
                Priority = responseData!.Priority;
            }
        }

        /// <summary>
        /// Request a key frame to the Producer.
        /// </summary>
        public async Task RequestKeyFrameAsync()
        {
            _logger.LogDebug($"RequestKeyFrameAsync() | Consumer:{ConsumerId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                await _channel.RequestAsync(MethodId.CONSUMER_REQUEST_KEY_FRAME, _internal);
            }
        }

        /// <summary>
        /// Enable 'trace' event.
        /// </summary>
        public async Task EnableTraceEventAsync(TraceEventType[] types)
        {
            _logger.LogDebug($"EnableTraceEventAsync() | Consumer:{ConsumerId}");

            var reqData = new
            {
                Types = types ?? Array.Empty<TraceEventType>()
            };

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Consumer closed");
                }

                // Fire and forget
                _channel.RequestAsync(MethodId.CONSUMER_ENABLE_TRACE_EVENT, _internal, reqData).ContinueWithOnFaultedHandleLog(_logger);
            }
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            _channel.MessageEvent += OnChannelMessage;
            _payloadChannel.MessageEvent += OnPayloadChannelMessage;
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void OnChannelMessage(string targetId, string @event, string? data)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            if (targetId != ConsumerId)
            {
                return;
            }

            switch (@event)
            {
                case "producerclose":
                    {
                        using (await _closeLock.WriteLockAsync())
                        {
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
                        }

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
