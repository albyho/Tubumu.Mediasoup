using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

namespace Tubumu.Mediasoup
{
    public class DataConsumer : EventEmitter
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<DataConsumer> _logger;

        /// <summary>
        /// Whether the DataConsumer is closed.
        /// </summary>
        private bool _closed;
        private readonly AsyncReaderWriterLock _closeLock = new();

        /// <summary>
        /// Internal data.
        /// </summary>
        private readonly DataConsumerInternal _internal;

        /// <summary>
        /// DataConsumer id.
        /// </summary>
        public string DataConsumerId => _internal.DataConsumerId;

        /// <summary>
        /// DataChannel data.
        /// </summary>
        public DataConsumerData Data { get; set; }

        /// <summary>
        /// Channel instance.
        /// </summary>
        private readonly IChannel _channel;

        /// <summary>
        /// App custom data.
        /// </summary>
        public Dictionary<string, object> AppData { get; }

        /// <summary>
        /// Observer instance.
        /// </summary>
        public EventEmitter Observer { get; } = new EventEmitter();

        /// <summary>
        /// <para>Events:</para>
        /// <para>@emits transportclose</para>
        /// <para>@emits dataproducerclose</para>
        /// <para>@emits message - (message: Buffer, ppid: number)</para>
        /// <para>@emits sctpsendbufferfull</para>
        /// <para>@emits bufferedamountlow - (bufferedAmount: number)</para>
        /// <para>@emits @close</para>
        /// <para>@emits @dataproducerclose</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="@internal"></param>
        /// <param name="data"></param>
        /// <param name="channel"></param>
        /// <param name="appData"></param>
        public DataConsumer(
            ILoggerFactory loggerFactory,
            DataConsumerInternal @internal,
            DataConsumerData data,
            IChannel channel,
            Dictionary<string, object>? appData
        )
        {
            _logger = loggerFactory.CreateLogger<DataConsumer>();

            _internal = @internal;
            Data = data;
            _channel = channel;
            AppData = appData ?? new Dictionary<string, object>();

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the DataConsumer.
        /// </summary>
        public async Task CloseAsync()
        {
            _logger.LogDebug("CloseAsync() | DataConsumer:{DataConsumerId}", DataConsumerId);

            using(await _closeLock.WriteLockAsync())
            {
                if(_closed)
                {
                    return;
                }

                _closed = true;

                // Remove notification subscriptions.
                _channel.OnNotification -= OnNotificationHandle;

                var reqData = new { DataConsumerId = _internal.DataConsumerId };

                // Fire and forget
                _channel
                    .RequestAsync(MethodId.TRANSPORT_CLOSE_DATA_CONSUMER, _internal.TransportId, reqData)
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
            _logger.LogDebug($"TransportClosedAsync() | DataConsumer:{DataConsumerId}");

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
        /// Dump DataConsumer.
        /// </summary>
        public async Task<string> DumpAsync()
        {
            _logger.LogDebug($"DumpAsync() | DataConsumer:{DataConsumerId}");

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                return (await _channel.RequestAsync(MethodId.DATA_CONSUMER_DUMP, _internal.DataConsumerId))!;
            }
        }

        /// <summary>
        /// Get DataConsumer stats. Return: DataConsumerStat[]
        /// </summary>
        public async Task<string> GetStatsAsync()
        {
            _logger.LogDebug($"GetStatsAsync() | DataConsumer:{DataConsumerId}");

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                return (await _channel.RequestAsync(MethodId.DATA_CONSUMER_GET_STATS, _internal.DataConsumerId))!;
            }
        }

        /// <summary>
        /// Set buffered amount low threshold.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task SetBufferedAmountLowThresholdAsync(uint threshold)
        {
            _logger.LogDebug($"SetBufferedAmountLowThreshold() | Threshold:{threshold}");

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                var reqData = new { Threshold = threshold };
                await _channel.RequestAsync(
                    MethodId.DATA_CONSUMER_SET_BUFFERED_AMOUNT_LOW_THRESHOLD,
                    _internal.DataConsumerId,
                    reqData
                );
            }
        }

        /// <summary>
        /// Send data (just valid for DataProducers created on a DirectTransport).
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ppid"></param>
        /// <returns></returns>
        public async Task SendAsync(string message, int? ppid)
        {
            _logger.LogDebug($"SendAsync() | DataConsumer:{DataConsumerId}");

            /*
             * +-------------------------------+----------+
             * | Value                         | SCTP     |
             * |                               | PPID     |
             * +-------------------------------+----------+
             * | WebRTC String                 | 51       |
             * | WebRTC Binary Partial         | 52       |
             * | (Deprecated)                  |          |
             * | WebRTC Binary                 | 53       |
             * | WebRTC String Partial         | 54       |
             * | (Deprecated)                  |          |
             * | WebRTC String Empty           | 56       |
             * | WebRTC Binary Empty           | 57       |
             * +-------------------------------+----------+
             */

            if(ppid == null)
            {
                ppid = !message.IsNullOrEmpty() ? 51 : 56;
            }

            // Ensure we honor PPIDs.
            if(ppid == 56)
            {
                message = " ";
            }

            var requestData = ppid.Value.ToString();

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                await _payloadChannel.NotifyAsync(
                    "dataConsumer.send",
                    _internal.DataConsumerId,
                    requestData,
                    Encoding.UTF8.GetBytes(message)
                );
            }
        }

        /// <summary>
        /// Send data (just valid for DataProducers created on a DirectTransport).
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ppid"></param>
        /// <returns></returns>
        public async Task SendAsync(byte[] message, int? ppid)
        {
            _logger.LogDebug($"SendAsync() | DataConsumer:{DataConsumerId}");

            if(ppid == null)
            {
                ppid = !message.IsNullOrEmpty() ? 53 : 57;
            }

            // Ensure we honor PPIDs.
            if(ppid == 57)
            {
                message = new byte[1];
            }

            var requestData = ppid.Value.ToString();

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                await _payloadChannel.NotifyAsync("dataConsumer.send", _internal.DataConsumerId, requestData, message);
            }
        }

        public async Task<string> GetBufferedAmountAsync()
        {
            _logger.LogDebug("GetBufferedAmountAsync()");

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                // 返回的是 JSON 格式，取其 bufferedAmount 属性。
                return (await _channel.RequestAsync(MethodId.DATA_CONSUMER_GET_BUFFERED_AMOUNT, _internal.DataConsumerId))!;
            }
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            _channel.OnNotification += OnNotificationHandle;
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void OnNotificationHandle(string targetId, string @event, string? data)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            if(targetId != DataConsumerId)
            {
                return;
            }

            switch(@event)
            {
                case "dataproducerclose":
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

                            Emit("@dataproducerclose");
                            Emit("dataproducerclose");

                            // Emit observer event.
                            Observer.Emit("close");
                        }
                        break;
                    }
                case "sctpsendbufferfull":
                    {
                        Emit("sctpsendbufferfull");

                        break;
                    }
                case "bufferedamount":
                    {
                        var bufferedAmount = int.Parse(data!);

                        Emit("bufferedamountlow", bufferedAmount);

                        break;
                    }
                default:
                    {
                        _logger.LogError($"OnNotificationHandle() | Ignoring unknown event \"{@event}\" in channel listener");
                        break;
                    }
            }
        }

        #endregion Event Handlers
    }
}
