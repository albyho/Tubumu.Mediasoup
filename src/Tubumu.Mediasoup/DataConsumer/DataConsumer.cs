using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tubumu.Utils.Extensions;
using Tubumu.Mediasoup.Extensions;

namespace Tubumu.Mediasoup
{
    public class DataConsumerInternalData
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
        /// Associated DataProducer id.
        /// </summary>
        public string DataProducerId { get; }

        /// <summary>
        /// DataConsumer id.
        /// </summary>
        public string DataConsumerId { get; }

        public DataConsumerInternalData(string routerId, string transportId, string dataProducerId, string dataConsumerId)
        {
            RouterId = routerId;
            TransportId = transportId;
            DataProducerId = dataProducerId;
            DataConsumerId = dataConsumerId;
        }
    }

    public class DataConsumer : EventEmitter
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<DataConsumer> _logger;

        // TODO: (alby) _closed 的使用及线程安全。
        /// <summary>
        /// Whether the DataConsumer is closed.
        /// </summary>
        private bool _closed;

        /// <summary>
        /// Internal data.
        /// </summary>
        private readonly DataConsumerInternalData _internal;

        /// <summary>
        /// DataConsumer id.
        /// </summary>
        public string DataConsumerId => _internal.DataConsumerId;

        #region DataConsumer data.

        /// <summary>
        /// SCTP stream parameters.
        /// </summary>
        public SctpStreamParameters? SctpStreamParameters { get; }

        /// <summary>
        /// DataChannel label.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// DataChannel protocol.
        /// </summary>
        public string Protocol { get; }

        #endregion DataConsumer data.

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
        /// <param name="dataConsumerInternalData"></param>
        /// <param name="sctpStreamParameters"></param>
        /// <param name="label"></param>
        /// <param name="protocol"></param>
        /// <param name="channel"></param>
        /// <param name="payloadChannel"></param>
        /// <param name="appData"></param>
        public DataConsumer(ILoggerFactory loggerFactory,
            DataConsumerInternalData dataConsumerInternalData,
            SctpStreamParameters sctpStreamParameters,
            string label,
            string protocol,
            IChannel channel,
            IPayloadChannel payloadChannel,
            Dictionary<string, object>? appData
            )
        {
            _logger = loggerFactory.CreateLogger<DataConsumer>();

            // Internal
            _internal = dataConsumerInternalData;

            // Data
            SctpStreamParameters = sctpStreamParameters;
            Label = label;
            Protocol = protocol;

            _channel = channel;
            _payloadChannel = payloadChannel;
            AppData = appData;

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the DataConsumer.
        /// </summary>
        public void Close()
        {
            if (_closed)
            {
                return;
            }

            _logger.LogDebug($"Close() | DataConsumer:{DataConsumerId}");

            _closed = true;

            // Remove notification subscriptions.
            _channel.MessageEvent -= OnChannelMessage;
            _payloadChannel.MessageEvent -= OnPayloadChannelMessage;

            // Fire and forget
            _channel.RequestAsync(MethodId.DATA_CONSUMER_CLOSE, _internal).ContinueWithOnFaultedHandleLog(_logger);

            Emit("@close");

            // Emit observer event.
            Observer.Emit("close");
        }

        /// <summary>
        /// Transport was closed.
        /// </summary>
        public void TransportClosed()
        {
            if (_closed)
            {
                return;
            }

            _logger.LogDebug($"Transport_closed() | DataConsumer:{DataConsumerId}");

            _closed = true;

            // Remove notification subscriptions.
            _channel.MessageEvent -= OnChannelMessage;
            _payloadChannel.MessageEvent -= OnPayloadChannelMessage;

            Emit("transportclose");

            // Emit observer event.
            Observer.Emit("close");
        }

        /// <summary>
        /// Dump DataConsumer.
        /// </summary>
        public Task<string?> DumpAsync()
        {
            _logger.LogDebug($"DumpAsync() | DataConsumer:{DataConsumerId}");

            return _channel.RequestAsync(MethodId.DATA_CONSUMER_DUMP, _internal);
        }

        /// <summary>
        /// Get DataConsumer stats. Return: DataConsumerStat[]
        /// </summary>
        public Task<string?> GetStatsAsync()
        {
            _logger.LogDebug($"GetStatsAsync() | DataConsumer:{DataConsumerId}");

            return _channel.RequestAsync(MethodId.DATA_CONSUMER_GET_STATS, _internal);
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

            var reqData = new { Threshold = threshold };
            await _channel.RequestAsync(MethodId.DATA_CONSUMER_SET_BUFFERED_AMOUNT_LOW_THRESHOLD, _internal, reqData);
        }

        /// <summary>
        /// Send data (just valid for DataProducers created on a DirectTransport).
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ppid"></param>
        /// <returns></returns>
        public Task SendAsync(string message, int? ppid)
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

            if (ppid == null)
            {
                ppid = !message.IsNullOrEmpty() ? 51 : 56;
            }

            // Ensure we honor PPIDs.
            if (ppid == 56)
            {
                message = " ";
            }

            var requestData = new NotifyData { PPID = ppid.Value };

            _payloadChannel.Notify("dataConsumer.send", _internal, requestData, Encoding.UTF8.GetBytes(message));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Send data (just valid for DataProducers created on a DirectTransport).
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ppid"></param>
        /// <returns></returns>
        public Task SendAsync(byte[] message, int? ppid)
        {
            _logger.LogDebug($"SendAsync() | DataConsumer:{DataConsumerId}");

            if (ppid == null)
            {
                ppid = !message.IsNullOrEmpty() ? 53 : 57;
            }

            // Ensure we honor PPIDs.
            if (ppid == 57)
            {
                message = new byte[1];
            }

            var requestData = new NotifyData { PPID = ppid.Value };

            _payloadChannel.Notify("dataConsumer.send", _internal, requestData, message);

            return Task.CompletedTask;
        }

        public Task<string?> GetBufferedAmountAsync()
        {
            _logger.LogDebug("GetBufferedAmountAsync()");

            // 返回的是 JSON 格式，取其 bufferedAmount 属性。
            return _channel.RequestAsync(MethodId.DATA_CONSUMER_GET_BUFFERED_AMOUNT, _internal);
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            _channel.MessageEvent += OnChannelMessage;
            _payloadChannel.MessageEvent += OnPayloadChannelMessage;
        }

        private void OnChannelMessage(string targetId, string @event, string? data)
        {
            if (targetId != DataConsumerId)
            {
                return;
            }

            switch (@event)
            {
                case "dataproducerclose":
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

                        Emit("@dataproducerclose");
                        Emit("dataproducerclose");

                        // Emit observer event.
                        Observer.Emit("close");

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
                        _logger.LogError($"OnChannelMessage() | Ignoring unknown event \"{@event}\" in channel listener");
                        break;
                    }
            }
        }

        private void OnPayloadChannelMessage(string targetId, string @event, NotifyData data, ArraySegment<byte> payload)
        {
            if (targetId != DataConsumerId)
            {
                return;
            }

            switch (@event)
            {
                case "message":
                    {
                        // TODO: (alby) _closed 的使用及线程安全。
                        if (_closed)
                        {
                            break;
                        }

                        var ppid = data.PPID;
                        var message = payload;

                        // Emit 暂不支持超过两个参数
                        Emit("message", new NotifyMessage { Message = message, PPID = ppid });

                        break;
                    }
                default:
                    {
                        _logger.LogError($"OnPayloadChannelMessage() | Ignoring unknown event \"{@event}\" in payload channel listener");
                        break;
                    }
            }
        }

        #endregion Event Handlers
    }
}
