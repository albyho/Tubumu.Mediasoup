using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

namespace Tubumu.Mediasoup
{
    public class Producer : EventEmitter
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<Producer> _logger;

        /// <summary>
        /// Whether the Producer is closed.
        /// </summary>
        public bool Closed { get; private set; }
        private readonly AsyncReaderWriterLock _closeLock = new();

        /// <summary>
        /// Paused flag.
        /// </summary>
        public bool Paused { get; private set; }
        private readonly AsyncAutoResetEvent _pauseLock = new();

        /// <summary>
        /// Internal data.
        /// </summary>
        private readonly ProducerInternal _internal;

        private readonly bool _isCheckConsumer = false;

        private readonly Timer? _checkConsumersTimer;

#if DEBUG
        private const int CheckConsumersTimeSeconds = 60 * 60 * 24;
#else
        private const int CheckConsumersTimeSeconds = 10;
#endif

        /// <summary>
        /// Producer id.
        /// </summary>
        public string ProducerId => _internal.ProducerId;

        /// <summary>
        /// Producer data.
        /// </summary>
        public ProducerData Data { get; set; }

        /// <summary>
        /// Channel instance.
        /// </summary>
        private readonly IChannel _channel;

        /// <summary>
        /// Channel instance.
        /// </summary>
        private readonly IPayloadChannel _payloadChannel;

        /// <summary>
        /// App custom data.
        /// </summary>
        public Dictionary<string, object> AppData { get; }

        /// <summary>
        /// [扩展]Consumers
        /// </summary>
        private readonly Dictionary<string, Consumer> _consumers = new();

        /// <summary>
        /// [扩展]Source.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Current score.
        /// </summary>
        public ProducerScore[] Score = Array.Empty<ProducerScore>();

        /// <summary>
        /// Observer instance.
        /// </summary>
        public EventEmitter Observer { get; } = new EventEmitter();

        /// <summary>
        /// <para>Events:</para>
        /// <para>@emits transportclose</para></para>
        /// <para>@emits score - (score: ProducerScore[])</para>
        /// <para>@emits videoorientationchange - (videoOrientation: ProducerVideoOrientation)</para>
        /// <para>@emits trace - (trace: ProducerTraceEventData)</para>
        /// <para>@emits @close</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// <para>@emits pause</para>
        /// <para>@emits resume</para>
        /// <para>@emits score - (score: ProducerScore[])</para>
        /// <para>@emits videoorientationchange - (videoOrientation: ProducerVideoOrientation)</para>
        /// <para>@emits trace - (trace: ProducerTraceEventData)</para>
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="@internal"></param>
        /// <param name="data"></param>
        /// <param name="channel"></param>
        /// <param name="appData"></param>
        /// <param name="paused"></param>
        public Producer(ILoggerFactory loggerFactory,
            ProducerInternal @internal,
            ProducerData data,
            IChannel channel,
            IPayloadChannel payloadChannel,
            Dictionary<string, object>? appData,
            bool paused
            )
        {
            _logger = loggerFactory.CreateLogger<Producer>();

            _internal = @internal;
            Data = data;
            _channel = channel;
            _payloadChannel = payloadChannel;
            AppData = appData ?? new Dictionary<string, object>();
            Paused = paused;
            _pauseLock.Set();

            if (_isCheckConsumer)
            {
                _checkConsumersTimer = new Timer(CheckConsumers, null, TimeSpan.FromSeconds(CheckConsumersTimeSeconds), TimeSpan.FromMilliseconds(-1));

            }
            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the Producer.
        /// </summary>
        public async Task CloseAsync()
        {
            _logger.LogDebug($"CloseAsync() | Producer:{ProducerId}");

            using(await _closeLock.WriteLockAsync())
            {
                CloseInternal();
            }
        }

        private void CloseInternal()
        {
            if (Closed)
            {
                return;
            }

            Closed = true;

            _checkConsumersTimer?.Dispose();

            // Remove notification subscriptions.
            _channel.MessageEvent -= OnChannelMessage;
            _payloadChannel.MessageEvent -= OnPayloadChannelMessage;

            var reqData = new { ProducerId = _internal.ProducerId };

            // Fire and forget
            _channel.RequestAsync(MethodId.TRANSPORT_CLOSE_PRODUCER, _internal.RouterId, reqData).ContinueWithOnFaultedHandleLog(_logger);

            Emit("@close");

            // Emit observer event.
            Observer.Emit("close");
        }

        /// <summary>
        /// Transport was closed.
        /// </summary>
        public async Task TransportClosedAsync()
        {
            _logger.LogDebug($"TransportClosedAsync() | Producer:{ProducerId}");

            using (await _closeLock.WriteLockAsync())
            {
                if (Closed)
                {
                    return;
                }

                Closed = true;

                if (_checkConsumersTimer != null)
                {
                  await _checkConsumersTimer.DisposeAsync();
                }

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
            _logger.LogDebug($"DumpAsync() | Producer:{ProducerId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (Closed)
                {
                    throw new InvalidStateException("Producer closed");
                }

                return (await _channel.RequestAsync(MethodId.PRODUCER_DUMP, _internal.ProducerId))!;
            }
        }

        /// <summary>
        /// Get DataProducer stats.
        /// </summary>
        public async Task<string> GetStatsAsync()
        {
            _logger.LogDebug($"GetStatsAsync() | Producer:{ProducerId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (Closed)
                {
                    throw new InvalidStateException("Producer closed");
                }

                return (await _channel.RequestAsync(MethodId.PRODUCER_GET_STATS, _internal.ProducerId))!;
            }
        }

        /// <summary>
        /// Pause the Producer.
        /// </summary>
        public async Task PauseAsync()
        {
            _logger.LogDebug($"PauseAsync() | Producer:{ProducerId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (Closed)
                {
                    throw new InvalidStateException("Producer closed");
                }

                await _pauseLock.WaitAsync();
                try
                {
                    var wasPaused = Paused;

                    await _channel.RequestAsync(MethodId.PRODUCER_PAUSE, _internal.ProducerId);

                    Paused = true;

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
        /// Resume the Producer.
        /// </summary>
        public async Task ResumeAsync()
        {
            _logger.LogDebug($"ResumeAsync() | Producer:{ProducerId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (Closed)
                {
                    throw new InvalidStateException("Producer closed");
                }

                await _pauseLock.WaitAsync();
                try
                {
                    var wasPaused = Paused;

                    await _channel.RequestAsync(MethodId.PRODUCER_RESUME, _internal.ProducerId);

                    Paused = false;

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
        /// Enable 'trace' event.
        /// </summary>
        public async Task EnableTraceEventAsync(TraceEventType[] types)
        {
            _logger.LogDebug($"EnableTraceEventAsync() | Producer:{ProducerId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (Closed)
                {
                    throw new InvalidStateException("Producer closed");
                }

                var reqData = new
                {
                    Types = types ?? Array.Empty<TraceEventType>()
                };

                await _channel.RequestAsync(MethodId.PRODUCER_ENABLE_TRACE_EVENT, _internal.ProducerId, reqData);
            }
        }

        /// <summary>
        /// Send RTP packet (just valid for Producers created on a DirectTransport).
        /// </summary>
        /// <param name="rtpPacket"></param>
        public async Task SendAsync(byte[] rtpPacket)
        {
            using (await _closeLock.ReadLockAsync())
            {
                if (Closed)
                {
                    throw new InvalidStateException("Producer closed");
                }

                await _payloadChannel.NotifyAsync("producer.send", _internal.ProducerId, null, rtpPacket);
            }
        }

        public async Task AddConsumerAsync(Consumer consumer)
        {
            using (await _closeLock.ReadLockAsync())
            {
                if (Closed)
                {
                    throw new InvalidStateException("Producer closed");
                }

                _consumers[consumer.ConsumerId] = consumer;
            }
        }

        public async Task RemoveConsumerAsync(string consumerId)
        {
            _logger.LogDebug($"RemoveConsumer() | Producer:{ProducerId} ConsumerId:{consumerId}");

            using (await _closeLock.ReadLockAsync())
            {
                // 关闭后也允许移除
                _consumers.Remove(consumerId);
            }
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            _channel.MessageEvent += OnChannelMessage;
            _payloadChannel.MessageEvent += OnPayloadChannelMessage;
        }

        private void OnChannelMessage(string targetId, string @event, string? data)
        {
            if (targetId != ProducerId)
            {
                return;
            }

            switch (@event)
            {
                case "score":
                    {
                        var score = JsonSerializer.Deserialize<ProducerScore[]>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;
                        Score = score;

                        Emit("score", score);

                        // Emit observer event.
                        Observer.Emit("score", score);

                        break;
                    }
                case "videoorientationchange":
                    {
                        var videoOrientation = JsonSerializer.Deserialize<ProducerVideoOrientation>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;

                        Emit("videoorientationchange", videoOrientation);

                        // Emit observer event.
                        Observer.Emit("videoorientationchange", videoOrientation);

                        break;
                    }
                case "trace":
                    {
                        var trace = JsonSerializer.Deserialize<TransportTraceEventData>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;

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

        private void OnPayloadChannelMessage(string targetId, string @event, string? data, ArraySegment<byte> payload)
        {
            if (targetId != ProducerId)
            {
                return;
            }

            switch (@event)
            {
                case "rtp":
                    {
                        Emit("rtp", payload);
                        //AppendAllBytes($"/Users/alby/Downloads/{targetId}.rtp", payload.Array!);
                        break;
                    }
                default:
                    {
                        _logger.LogError($"OnPayloadChannelMessage() | Ignoring unknown event{@event}");
                        break;
                    }
            }
        }

        private static void AppendAllBytes(string path, byte[] bytes)
        {
            //argument-checking here.

            using (var stream = new FileStream(path, FileMode.Append))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        #endregion Event Handlers

        #region Private Methods

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void CheckConsumers(object? state)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            _logger.LogDebug($"CheckConsumer() | Producer: {_internal.ProducerId} Consumers: {_consumers.Count}");

            // NOTE: 使用写锁
            using (await _closeLock.WriteLockAsync())
            {
                if (Closed)
                {
                    _checkConsumersTimer?.Dispose();
                    return;
                }

                if (_consumers.Count == 0)
                {
                    CloseInternal();
                    _checkConsumersTimer?.Dispose();
                }
                else
                {
                    _checkConsumersTimer?.Change(TimeSpan.FromSeconds(CheckConsumersTimeSeconds), TimeSpan.FromMilliseconds(-1));
                }
            }
        }

        #endregion Private Methods
    }
}
