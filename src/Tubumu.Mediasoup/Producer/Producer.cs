using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FBS.Notification;
using FBS.Producer;
using FBS.Request;
using FBS.RtpStream;
using Google.FlatBuffers;
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
        public List<ScoreT> Score = new(0);

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
        public Producer(
            ILoggerFactory loggerFactory,
            ProducerInternal internal_,
            ProducerData data,
            IChannel channel,
            Dictionary<string, object>? appData,
            bool paused
        )
        {
            _logger = loggerFactory.CreateLogger<Producer>();

            _internal = internal_;
            Data = data;
            _channel = channel;
            AppData = appData ?? new Dictionary<string, object>();
            Paused = paused;
            _pauseLock.Set();

            if(_isCheckConsumer)
            {
                _checkConsumersTimer = new Timer(
                    CheckConsumers,
                    null,
                    TimeSpan.FromSeconds(CheckConsumersTimeSeconds),
                    TimeSpan.FromMilliseconds(-1)
                );
            }

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the Producer.
        /// </summary>
        public async Task CloseAsync()
        {
            _logger.LogDebug("CloseAsync() | Producer:{ProducerId}", ProducerId);

            using(await _closeLock.WriteLockAsync())
            {
                CloseInternal();
            }
        }

        private void CloseInternal()
        {
            if(Closed)
            {
                return;
            }

            Closed = true;

            _checkConsumersTimer?.Dispose();

            // Remove notification subscriptions.
            _channel.OnNotification -= OnNotificationHandle;

            // Build Request
            var bufferBuilder = _channel.BufferPool.Get();

            var requestOffset = FBS.Transport.CloseProducerRequest.Pack(bufferBuilder, new FBS.Transport.CloseProducerRequestT
            {
                ProducerId = _internal.ProducerId
            });

            // Fire and forget
            _channel.RequestAsync(bufferBuilder, Method.TRANSPORT_CLOSE_CONSUMER,
                FBS.Request.Body.Transport_CloseConsumerRequest,
                requestOffset.Value,
                _internal.TransportId
                )
                .ContinueWithOnFaultedHandleLog(_logger);

            Emit("@close");

            // Emit observer event.
            Observer.Emit("close");
        }

        /// <summary>
        /// Transport was closed.
        /// </summary>
        public async Task TransportClosedAsync()
        {
            _logger.LogDebug("TransportClosedAsync() | Producer:{ProducerId}", ProducerId);

            using(await _closeLock.WriteLockAsync())
            {
                if(Closed)
                {
                    return;
                }

                Closed = true;

                if(_checkConsumersTimer != null)
                {
                    await _checkConsumersTimer.DisposeAsync();
                }

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
            _logger.LogDebug("DumpAsync() | Producer:{ProducerId}", ProducerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(Closed)
                {
                    throw new InvalidStateException("Producer closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var response = await _channel.RequestAsync(bufferBuilder, Method.PRODUCER_DUMP, null, null, _internal.ProducerId);
                var data = response.Value.BodyAsProducer_DumpResponse().UnPack();

                return data;
            }
        }

        /// <summary>
        /// Get DataProducer stats.
        /// </summary>
        public async Task<List<StatsT>> GetStatsAsync()
        {
            _logger.LogDebug("GetStatsAsync() | Producer:{ProducerId}", ProducerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(Closed)
                {
                    throw new InvalidStateException("Producer closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var response = await _channel.RequestAsync(bufferBuilder, Method.PRODUCER_GET_STATS, null, null, _internal.ProducerId);
                var stats = response.Value.BodyAsProducer_GetStatsResponse().UnPack().Stats;

                return stats;
            }
        }

        /// <summary>
        /// Pause the Producer.
        /// </summary>
        public async Task PauseAsync()
        {
            _logger.LogDebug("PauseAsync() | Producer:{ProducerId}", ProducerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(Closed)
                {
                    throw new InvalidStateException("Producer closed");
                }

                await _pauseLock.WaitAsync();
                try
                {
                    var wasPaused = Paused;

                    // Build Request
                    var bufferBuilder = _channel.BufferPool.Get();

                    await _channel.RequestAsync(bufferBuilder, Method.PRODUCER_PAUSE, null, null, _internal.ProducerId);

                    Paused = true;

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
        /// Resume the Producer.
        /// </summary>
        public async Task ResumeAsync()
        {
            _logger.LogDebug("ResumeAsync() | Producer:{ProducerId}", ProducerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(Closed)
                {
                    throw new InvalidStateException("Producer closed");
                }

                await _pauseLock.WaitAsync();
                try
                {
                    var wasPaused = Paused;

                    // Build Request
                    var bufferBuilder = _channel.BufferPool.Get();

                    await _channel.RequestAsync(bufferBuilder, Method.PRODUCER_RESUME, null, null, _internal.ProducerId);

                    Paused = false;

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
        /// Enable 'trace' event.
        /// </summary>
        public async Task EnableTraceEventAsync(List<TraceEventType> types)
        {
            _logger.LogDebug("EnableTraceEventAsync() | Producer:{ProducerId}", ProducerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(Closed)
                {
                    throw new InvalidStateException("Producer closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var requestOffset = EnableTraceEventRequest.Pack(bufferBuilder, new EnableTraceEventRequestT
                {
                    Events = types ?? new List<TraceEventType>(0)
                });

                // Fire and forget
                _channel.RequestAsync(bufferBuilder, Method.CONSUMER_ENABLE_TRACE_EVENT,
                    FBS.Request.Body.Consumer_EnableTraceEventRequest,
                    requestOffset.Value,
                     _internal.ProducerId)
                    .ContinueWithOnFaultedHandleLog(_logger);
            }
        }

        /// <summary>
        /// Send RTP packet (just valid for Producers created on a DirectTransport).
        /// </summary>
        public async Task SendAsync(byte[] rtpPacket)
        {
            using(await _closeLock.ReadLockAsync())
            {
                if(Closed)
                {
                    throw new InvalidStateException("Producer closed");
                }

                // Build Request
                var bufferBuilder = new FlatBufferBuilder(1024 + rtpPacket.Length);

                var dataOffset = SendNotification.CreateDataVectorBlock(
                    bufferBuilder,
                    rtpPacket
                );

                var notificationOffset = SendNotification.CreateSendNotification(bufferBuilder, dataOffset);

                // Fire and forget
                _channel.NotifyAsync(bufferBuilder, Event.PRODUCER_SEND,
                    FBS.Notification.Body.Producer_SendNotification,
                    notificationOffset.Value,
                    _internal.ProducerId
                ).ContinueWithOnFaultedHandleLog(_logger);
            }
        }

        public async Task AddConsumerAsync(Consumer consumer)
        {
            using(await _closeLock.ReadLockAsync())
            {
                if(Closed)
                {
                    throw new InvalidStateException("Producer closed");
                }

                _consumers[consumer.ConsumerId] = consumer;
            }
        }

        public async Task RemoveConsumerAsync(string consumerId)
        {
            _logger.LogDebug("RemoveConsumer() | Producer:{ProducerId} ConsumerId:{ConsumerId}", ProducerId, consumerId);

            using(await _closeLock.ReadLockAsync())
            {
                // 关闭后也允许移除
                _consumers.Remove(consumerId);
            }
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            _channel.OnNotification += OnNotificationHandle;
        }

        private void OnNotificationHandle(string handlerId, Event @event, Notification notification)
        {
            if(handlerId != ProducerId)
            {
                return;
            }

            switch(@event)
            {
                case Event.PRODUCER_SCORE:
                    {
                        var scoreNotification = notification.BodyAsProducer_ScoreNotification();
                        var score = scoreNotification.UnPack().Scores;
                        Score = score;

                        Emit("score", score);

                        // Emit observer event.
                        Observer.Emit("score", score);

                        break;
                    }
                case Event.PRODUCER_VIDEO_ORIENTATION_CHANGE:
                    {
                        var videoOrientationChangeNotification = notification.BodyAsProducer_VideoOrientationChangeNotification();
                        var videoOrientation = videoOrientationChangeNotification.UnPack();

                        Emit("videoorientationchange", videoOrientation);

                        // Emit observer event.
                        Observer.Emit("videoorientationchange", videoOrientation);

                        break;
                    }
                case Event.PRODUCER_TRACE:
                    {
                        var traceNotification = notification.BodyAsProducer_TraceNotification();
                        var trace = traceNotification.UnPack();

                        Emit("trace", trace);

                        // Emit observer event.
                        Observer.Emit("trace", trace);

                        break;
                    }
                default:
                    {
                        _logger.LogError("OnNotificationHandle() | Ignoring unknown event: {@event}", @event);
                        break;
                    }
            }
        }

        #endregion Event Handlers

        #region Private Methods

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void CheckConsumers(object? state)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            _logger.LogDebug("CheckConsumer() | Producer:{ProducerId} ConsumerCount:{Count}", _internal.ProducerId, _consumers.Count);

            // NOTE: 使用写锁
            using(await _closeLock.WriteLockAsync())
            {
                if(Closed)
                {
                    _checkConsumersTimer?.Dispose();
                    return;
                }

                if(_consumers.Count == 0)
                {
                    CloseInternal();
                    _checkConsumersTimer?.Dispose();
                }
                else
                {
                    _checkConsumersTimer?.Change(
                        TimeSpan.FromSeconds(CheckConsumersTimeSeconds),
                        TimeSpan.FromMilliseconds(-1)
                    );
                }
            }
        }

        #endregion Private Methods
    }
}
