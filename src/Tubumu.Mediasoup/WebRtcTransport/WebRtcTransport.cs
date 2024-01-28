using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FBS.Notification;
using FBS.Request;
using FBS.WebRtcTransport;
using Microsoft.Extensions.Logging;

namespace Tubumu.Mediasoup
{
    public class WebRtcTransport : Transport
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<WebRtcTransport> _logger;

        public DumpResponseT Data { get; }

        /// <summary>
        /// <para>Events:</para>
        /// <para>@emits icestatechange - (iceState: IceState)</para>
        /// <para>@emits iceselectedtuplechange - (iceSelectedTuple: TransportTuple)</para>
        /// <para>@emits dtlsstatechange - (dtlsState: DtlsState)</para>
        /// <para>@emits sctpstatechange - (sctpState: SctpState)</para>
        /// <para>@emits trace - (trace: TransportTraceEventData)</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// <para>@emits newproducer - (producer: Producer)</para>
        /// <para>@emits newconsumer - (consumer: Consumer)</para>
        /// <para>@emits newdataproducer - (dataProducer: DataProducer)</para>
        /// <para>@emits newdataconsumer - (dataConsumer: DataConsumer)</para>
        /// <para>@emits icestatechange - (iceState: IceState)</para>
        /// <para>@emits iceselectedtuplechange - (iceSelectedTuple: TransportTuple)</para>
        /// <para>@emits dtlsstatechange - (dtlsState: DtlsState)</para>
        /// <para>@emits sctpstatechange - (sctpState: SctpState)</para>
        /// <para>@emits trace - (trace: TransportTraceEventData)</para>
        /// </summary>
        public WebRtcTransport(
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
            _logger = loggerFactory.CreateLogger<WebRtcTransport>();

            Data = data;

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the WebRtcTransport.
        /// </summary>
        protected override Task OnCloseAsync()
        {
            Data.IceState = IceState.DISCONNECTED; // CLOSED
            Data.IceSelectedTuple = null;
            Data.DtlsState = DtlsState.CLOSED;

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
            var bufferBuilder = Channel.BufferPool.Get();

            var response = await Channel.RequestAsync(bufferBuilder, Method.TRANSPORT_DUMP, null, null, Internal.TransportId);
            var data = response.Value.BodyAsWebRtcTransport_DumpResponse().UnPack();

            return data;
        }

        /// <summary>
        /// Get Transport stats.
        /// </summary>
        protected override async Task<object[]> OnGetStatsAsync()
        {
            // Build Request
            var bufferBuilder = Channel.BufferPool.Get();

            var response = await Channel.RequestAsync(bufferBuilder, Method.TRANSPORT_GET_STATS, null, null, Internal.TransportId);
            var data = response.Value.BodyAsWebRtcTransport_GetStatsResponse().UnPack();

            return new[] { data };
        }

        /// <summary>
        /// Provide the WebRtcTransport remote parameters.
        /// </summary>
        protected override async Task OnConnectAsync(object parameters)
        {
            _logger.LogDebug("OnConnectAsync() | WebRtcTransportId:{WebRtcTransportId}", TransportId);

            if(parameters is not ConnectRequestT connectRequestT)
            {
                throw new Exception($"{nameof(parameters)} type is not FBS.WebRtcTransport.ConnectRequestT");
            }

            // Build Request
            var bufferBuilder = Channel.BufferPool.Get();

            var connectRequestOffset = ConnectRequest.Pack(bufferBuilder, connectRequestT);

            var response = await Channel.RequestAsync(bufferBuilder, Method.WEBRTCTRANSPORT_CONNECT,
                 FBS.Request.Body.WebRtcTransport_ConnectRequest,
                 connectRequestOffset.Value,
                 Internal.TransportId);

            /* Decode Response. */
            var data = response.Value.BodyAsWebRtcTransport_ConnectResponse().UnPack();

            // Update data.
            Data.DtlsParameters.Role = data.DtlsLocalRole;
        }

        /// <summary>
        /// Restart ICE.
        /// </summary>
        public async Task<IceParametersT> RestartIceAsync()
        {
            _logger.LogDebug("RestartIceAsync() | WebRtcTransportId:{WebRtcTransportId}", TransportId);

            using(await CloseLock.ReadLockAsync())
            {
                if(Closed)
                {
                    throw new InvalidStateException("Transport closed");
                }

                // Build Request
                var bufferBuilder = Channel.BufferPool.Get();

                var response = await Channel.RequestAsync(bufferBuilder, Method.TRANSPORT_RESTART_ICE,
                     null,
                     null,
                     Internal.TransportId);

                /* Decode Response. */
                var data = response.Value.BodyAsTransport_RestartIceResponse().UnPack();

                // Update data.
                Data.IceParameters = new IceParametersT
                {
                    UsernameFragment = data.UsernameFragment,
                    Password = data.Password,
                    IceLite = data.IceLite,
                };

                return Data.IceParameters;
            }
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
                case Event.WEBRTCTRANSPORT_ICE_STATE_CHANGE:
                    {
                        var iceStateChangeNotification = notification.BodyAsWebRtcTransport_IceStateChangeNotification().UnPack();

                        Data.IceState = iceStateChangeNotification.IceState;

                        Emit("icestatechange", Data.IceState);

                        // Emit observer event.
                        Observer.Emit("icestatechange", Data.IceState);

                        break;
                    }
                case Event.WEBRTCTRANSPORT_ICE_SELECTED_TUPLE_CHANGE:
                    {
                        var iceSelectedTupleChangeNotification = notification.BodyAsWebRtcTransport_IceSelectedTupleChangeNotification().UnPack();

                        Data.IceSelectedTuple = iceSelectedTupleChangeNotification.Tuple;

                        Emit("iceselectedtuplechange", Data.IceSelectedTuple);

                        // Emit observer event.
                        Observer.Emit("iceselectedtuplechange", Data.IceSelectedTuple);

                        break;
                    }

                case Event.WEBRTCTRANSPORT_DTLS_STATE_CHANGE:
                    {
                        var dtlsStateChangeNotification = notification.BodyAsWebRtcTransport_DtlsStateChangeNotification().UnPack();

                        Data.DtlsState = dtlsStateChangeNotification.DtlsState;

                        if(Data.DtlsState == DtlsState.CONNECTED)
                        {
                            // TODO: DtlsRemoteCert donot exists.
                            // Data.DtlsRemoteCert = dtlsStateChangeNotification.RemoteCert;
                        }

                        Emit("dtlsstatechange", Data.DtlsState);

                        // Emit observer event.
                        Observer.Emit("dtlsstatechange", Data.DtlsState);

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
                        _logger.LogError("OnNotificationHandle() | WebRtcTransport:{TransportId} Ignoring unknown event:{@event}", TransportId, @event);
                        break;
                    }
            }
        }

        #endregion Event Handlers
    }
}
