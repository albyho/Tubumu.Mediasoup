using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FBS.Notification;
using FBS.PlainTransport;
using FBS.Request;
using Google.FlatBuffers;
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
        public DumpResponseT Data { get; }

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
        /// <param name="internal_"></param>
        /// <param name="data"></param>
        /// <param name="channel"></param>
        /// <param name="appData"></param>
        /// <param name="getRouterRtpCapabilities"></param>
        /// <param name="getProducerById"></param>
        /// <param name="getDataProducerById"></param>
        public PlainTransport(
            ILoggerFactory loggerFactory,
            TransportInternal internal_,
            DumpResponseT data,
            IChannel channel,
            Dictionary<string, object>? appData,
            Func<RtpCapabilities> getRouterRtpCapabilities,
            Func<string, Task<Producer?>> getProducerById,
            Func<string, Task<DataProducer?>> getDataProducerById
        )
            : base(
                loggerFactory,
                internal_,
                data.Base,
                channel,
                appData,
                getRouterRtpCapabilities,
                getProducerById,
                getDataProducerById
            )
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
            if(Data.Base.SctpState.HasValue)
            {
                Data.Base.SctpState = FBS.SctpAssociation.SctpState.CLOSED;
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
        /// Dump Transport.
        /// </summary>
        protected override async Task<object> OnDumpAsync()
        {
            // Build Request
            var bufferBuilder = new FlatBufferBuilder(1024);

            var response = await Channel.RequestAsync(bufferBuilder, Method.TRANSPORT_DUMP, null, null, Internal.TransportId);
            var data = response.Value.BodyAsPlainTransport_DumpResponse().UnPack();

            return data;
        }

        /// <summary>
        /// Get Transport stats.
        /// </summary>
        protected override async Task<object[]> OnGetStatsAsync()
        {
            // Build Request
            var bufferBuilder = new FlatBufferBuilder(1024);

            var response = await Channel.RequestAsync(bufferBuilder, Method.TRANSPORT_GET_STATS, null, null, Internal.TransportId);
            var data = response.Value.BodyAsPlainTransport_GetStatsResponse().UnPack();

            return new[] { data };
        }

        /// <summary>
        /// Provide the PlainTransport remote parameters.
        /// </summary>
        protected override async Task OnConnectAsync(object parameters)
        {
            _logger.LogDebug("OnConnectAsync() | PlainTransport:{TransportId}", TransportId);

            if(parameters is not ConnectRequestT connectRequestT)
            {
                throw new Exception($"{nameof(parameters)} type is not FBS.PlainTransport.ConnectRequestT");
            }

            // Build Request
            var bufferBuilder = new FlatBufferBuilder(1024);

            var connectRequestOffset = ConnectRequest.Pack(bufferBuilder, connectRequestT);

            var response = await Channel.RequestAsync(bufferBuilder, Method.PLAINTRANSPORT_CONNECT,
                 FBS.Request.Body.PlainTransport_ConnectRequest,
                 connectRequestOffset.Value,
                 Internal.TransportId);

            /* Decode Response. */
            var data = response.Value.BodyAsPlainTransport_ConnectResponse().UnPack();

            // Update data.
            if(data.Tuple != null)
            {
                Data.Tuple = data.Tuple;
            }

            if(data.RtcpTuple != null)
            {
                Data.RtcpTuple = data.RtcpTuple;
            }

            Data.SrtpParameters = data.SrtpParameters;
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            Channel.OnNotification += OnNotificationHandle;
        }

        private void OnNotificationHandle(string handlerId, Event @event, Notification notification)
        {
            if(handlerId != Internal.TransportId)
            {
                return;
            }

            switch(@event)
            {
                case Event.PLAINTRANSPORT_TUPLE:
                    {
                        var tupleNotification = notification.BodyAsPlainTransport_TupleNotification().UnPack();

                        Data.Tuple = tupleNotification.Tuple;

                        Emit("tuple", Data.Tuple);

                        // Emit observer event.
                        Observer.Emit("tuple", Data.Tuple);

                        break;
                    }
                case Event.PLAINTRANSPORT_RTCP_TUPLE:
                    {
                        var rtcpTupleNotification = notification.BodyAsPlainTransport_RtcpTupleNotification().UnPack();

                        Data.RtcpTuple = rtcpTupleNotification.Tuple;

                        Emit("rtcptuple", Data.RtcpTuple);

                        // Emit observer event.
                        Observer.Emit("rtcptuple", Data.RtcpTuple);

                        break;
                    }
                case Event.TRANSPORT_SCTP_STATE_CHANGE:
                    {
                        var sctpStateChangeNotification = notification.BodyAsTransport_SctpStateChangeNotification().UnPack();

                        Data.Base.SctpState = sctpStateChangeNotification.SctpState;

                        Emit("sctpstatechange", Data.Base.SctpState);

                        // Emit observer event.
                        Observer.Emit("sctpstatechange", Data.Base.SctpState);

                        break;
                    }
                case Event.TRANSPORT_TRACE:
                    {
                        var traceNotification = notification.BodyAsTransport_TraceNotification().UnPack();

                        Emit("trace", traceNotification);

                        // Emit observer event.
                        Observer.Emit("trace", traceNotification);

                        break;
                    }
                default:
                    {
                        _logger.LogError("OnNotificationHandle() | PlainTransport:{TransportId} Ignoring unknown event:{@event}", TransportId, @event);
                        break;
                    }
            }
        }

        #endregion Event Handlers
    }
}
