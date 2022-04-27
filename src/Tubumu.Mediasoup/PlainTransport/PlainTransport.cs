using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ObjectExtensions = Tubumu.Utils.Extensions.Object.ObjectExtensions;

namespace Tubumu.Mediasoup
{
    public class PlainTransport : Transport
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<PlainTransport> _logger;

        #region Producer data.

        public bool? RtcpMux { get; set; }

        public bool? Comedia { get; set; }

        public TransportTuple Tuple { get; private set; }

        public TransportTuple? RtcpTuple { get; private set; }

        public SrtpParameters? SrtpParameters { get; private set; }

        #endregion Producer data.

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
        /// <param name="transportInternalData"></param>
        /// <param name="sctpParameters"></param>
        /// <param name="sctpState"></param>
        /// <param name="channel"></param>
        /// <param name="payloadChannel"></param>
        /// <param name="appData"></param>
        /// <param name="getRouterRtpCapabilities"></param>
        /// <param name="getProducerById"></param>
        /// <param name="getDataProducerById"></param>
        public PlainTransport(ILoggerFactory loggerFactory,
            TransportInternalData transportInternalData,
            SctpParameters? sctpParameters,
            SctpState? sctpState,
            Channel channel,
            PayloadChannel payloadChannel,
            Dictionary<string, object>? appData,
            Func<RtpCapabilities> getRouterRtpCapabilities,
            Func<string, Producer?> getProducerById,
            Func<string, DataProducer?> getDataProducerById,
            bool? rtcpMux,
            bool? comedia,
            TransportTuple tuple,
            TransportTuple? rtcpTuple,
            SrtpParameters? srtpParameters
            ) : base(loggerFactory, transportInternalData, sctpParameters, sctpState, channel, payloadChannel, appData, getRouterRtpCapabilities, getProducerById, getDataProducerById)
        {
            _logger = loggerFactory.CreateLogger<PlainTransport>();

            // Data
            RtcpMux = rtcpMux;
            Comedia = comedia;
            Tuple = tuple;
            RtcpTuple = rtcpTuple;
            SrtpParameters = srtpParameters;

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the PlainTransport.
        /// </summary>
        public override async Task CloseAsync()
        {
            if (Closed)
            {
                return;
            }

            await CloseLock.WaitAsync();
            try
            {
                if (Closed)
                {
                    return;
                }

                if (SctpState.HasValue)
                {
                    SctpState = Mediasoup.SctpState.Closed;
                }

                await base.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CloseAsync()");
            }
            finally
            {
                CloseLock.Set();
            }
        }

        /// <summary>
        /// Router was closed.
        /// </summary>
        public override async Task RouterClosedAsync()
        {
            if (Closed)
            {
                return;
            }

            await CloseLock.WaitAsync();
            try
            {
                if (Closed)
                {
                    return;
                }

                if (SctpState.HasValue)
                {
                    SctpState = Mediasoup.SctpState.Closed;
                }

                await base.RouterClosedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RouterClosedAsync()");
            }
            finally
            {
                CloseLock.Set();
            }
        }

        /// <summary>
        /// Provide the PipeTransport remote parameters.
        /// </summary>
        public override Task ConnectAsync(object parameters)
        {
            _logger.LogDebug("ConnectAsync()");

            return parameters is PlainTransportConnectParameters connectParameters
                ? ConnectAsync(connectParameters)
                : throw new Exception($"{nameof(parameters)} type is not PipTransportConnectParameters");
        }

        private async Task ConnectAsync(PlainTransportConnectParameters plainTransportConnectParameters)
        {
            var reqData = plainTransportConnectParameters;

            var resData = await Channel.RequestAsync(MethodId.TRANSPORT_CONNECT, Internal, reqData);
            var responseData = JsonSerializer.Deserialize<PlainTransportConnectResponseData>(resData!, ObjectExtensions.DefaultJsonSerializerOptions)!;

            // Update data.
            if (responseData.Tuple != null)
            {
                Tuple = responseData.Tuple;
            }

            if (responseData.RtcpTuple != null)
            {
                RtcpTuple = responseData.RtcpTuple;
            }

            SrtpParameters = responseData.SrtpParameters;
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

                        Tuple = notification.Tuple;

                        Emit("tuple", Tuple);

                        // Emit observer event.
                        Observer.Emit("tuple", Tuple);

                        break;
                    }

                case "rtcptuple":
                    {
                        var notification = JsonSerializer.Deserialize<PlainTransportRtcpTupleNotificationData>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;

                        RtcpTuple = notification.RtcpTuple;

                        Emit("rtcptuple", RtcpTuple);

                        // Emit observer event.
                        Observer.Emit("rtcptuple", RtcpTuple);

                        break;
                    }

                case "sctpstatechange":
                    {
                        var notification = JsonSerializer.Deserialize<TransportSctpStateChangeNotificationData>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;

                        SctpState = notification.SctpState;

                        Emit("sctpstatechange", SctpState);

                        // Emit observer event.
                        Observer.Emit("sctpstatechange", SctpState);

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
