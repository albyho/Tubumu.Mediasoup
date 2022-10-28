using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tubumu.Mediasoup
{
    public class PlainTransport : Transport
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<PlainTransport> _logger;

        /// <summary>
        /// Producer data.
        /// </summary>
        public PlainTransportData Data { get; set; }

        /// <summary>
        /// <para>Events:</para>
        /// <para>@emits tuple - (tuple: TransportTuple)</para>
        /// <para>@emits rtcptuple - (rtcpTuple: TransportTuple)</para>
        /// <para>@emits sctpstatechange - (sctpState: SctpState)</para>
        /// <para>@emits trace - (trace: TransportTraceEventData)</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// <para>@emits newproducer - (producer: Producer)</para>
        /// <para>@emits newconsumer - (consumer: Consumer)</para>
        /// <para>@emits newdataproducer - (dataProducer: DataProducer)</para>
        /// <para>@emits newdataconsumer - (dataConsumer: DataConsumer)</para>
        /// <para>@emits tuple - (tuple: TransportTuple)</para>
        /// <para>@emits rtcptuple - (rtcpTuple: TransportTuple)</para>
        /// <para>@emits sctpstatechange - (sctpState: SctpState)</para>
        /// <para>@emits trace - (trace: TransportTraceEventData)</para>
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="@internal"></param>
        /// <param name="data"></param>
        /// <param name="channel"></param>
        /// <param name="payloadChannel"></param>
        /// <param name="appData"></param>
        /// <param name="getRouterRtpCapabilities"></param>
        /// <param name="getProducerById"></param>
        /// <param name="getDataProducerById"></param>
        public PlainTransport(ILoggerFactory loggerFactory,
            TransportInternal @internal,
            PlainTransportData data,
            IChannel channel,
            IPayloadChannel payloadChannel,
            Dictionary<string, object>? appData,
            Func<RtpCapabilities> getRouterRtpCapabilities,
            Func<string, Task<Producer?>> getProducerById,
            Func<string, Task<DataProducer?>> getDataProducerById
            ) : base(loggerFactory, @internal, data, channel, payloadChannel, appData, getRouterRtpCapabilities, getProducerById, getDataProducerById)
        {
            _logger = loggerFactory.CreateLogger<PlainTransport>();

            Data = data;

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the PlainTransport.
        /// </summary>
        protected override Task OnCloseAsync()
        {
            if (Data.SctpState.HasValue)
            {
                Data.SctpState = SctpState.Closed;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Router was closed.
        /// </summary>
        protected override Task OnRouterClosedAsync()
        {
            return OnCloseAsync();
        }

        /// <summary>
        /// Provide the PipeTransport remote parameters.
        /// </summary>
        public override async Task ConnectAsync(object parameters)
        {
            _logger.LogDebug("ConnectAsync()");

            if (parameters is not PlainTransportConnectParameters connectParameters)
            {
                throw new Exception($"{nameof(parameters)} type is not PlainTransportConnectParameters");
            }

            await ConnectAsync(connectParameters);
        }

        private async Task ConnectAsync(PlainTransportConnectParameters plainTransportConnectParameters)
        {
            using (await CloseLock.ReadLockAsync())
            {
                if (Closed)
                {
                    throw new InvalidStateException("Transport closed");
                }

                var reqData = plainTransportConnectParameters;
                var resData = await Channel.RequestAsync(MethodId.TRANSPORT_CONNECT, Internal.TransportId, reqData);
                var responseData = JsonSerializer.Deserialize<PlainTransportConnectResponseData>(resData!, ObjectExtensions.DefaultJsonSerializerOptions)!;

                // Update data.
                if (responseData.Tuple != null)
                {
                    Data.Tuple = responseData.Tuple;
                }

                if (responseData.RtcpTuple != null)
                {
                    Data.RtcpTuple = responseData.RtcpTuple;
                }

                Data.SrtpParameters = responseData.SrtpParameters;
            }
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            Channel.MessageEvent += OnChannelMessage;
        }

        private void OnChannelMessage(string targetId, string @event, string? data)
        {
            if (targetId != Internal.TransportId)
            {
                return;
            }

            switch (@event)
            {
                case "tuple":
                    {
                        var notification = JsonSerializer.Deserialize<PlainTransportTupleNotificationData>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;

                        Data.Tuple = notification.Tuple;

                        Emit("tuple", Data.Tuple);

                        // Emit observer event.
                        Observer.Emit("tuple", Data.Tuple);

                        break;
                    }

                case "rtcptuple":
                    {
                        var notification = JsonSerializer.Deserialize<PlainTransportRtcpTupleNotificationData>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;

                        Data.RtcpTuple = notification.RtcpTuple;

                        Emit("rtcptuple", Data.RtcpTuple);

                        // Emit observer event.
                        Observer.Emit("rtcptuple", Data.RtcpTuple);

                        break;
                    }

                case "sctpstatechange":
                    {
                        var notification = JsonSerializer.Deserialize<TransportSctpStateChangeNotificationData>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;

                        Data.SctpState = notification.SctpState;

                        Emit("sctpstatechange", Data.SctpState);

                        // Emit observer event.
                        Observer.Emit("sctpstatechange", Data.SctpState);

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

        #endregion Event Handlers
    }
}
