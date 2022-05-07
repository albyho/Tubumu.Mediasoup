using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tubumu.Mediasoup
{
    public class WebRtcTransport : Transport
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<WebRtcTransport> _logger;

        #region WebRtcTransport data.

        public string IceRole { get; private set; } = "controlled";

        public IceParameters IceParameters { get; private set; }

        public IceCandidate[] IceCandidates { get; private set; }

        public IceState IceState { get; private set; }

        public TransportTuple? IceSelectedTuple { get; private set; }

        public DtlsParameters DtlsParameters { get; private set; }

        public DtlsState DtlsState { get; private set; }

        public string? DtlsRemoteCert { get; private set; }

        #endregion WebRtcTransport data.

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
        /// <param name="iceRole"></param>
        /// <param name="iceParameters"></param>
        /// <param name="iceCandidates"></param>
        /// <param name="iceState"></param>
        /// <param name="iceSelectedTuple"></param>
        /// <param name="dtlsParameters"></param>
        /// <param name="dtlsState"></param>
        /// <param name="dtlsRemoteCert"></param>
        public WebRtcTransport(ILoggerFactory loggerFactory,
            TransportInternalData transportInternalData,
            SctpParameters? sctpParameters,
            SctpState? sctpState,
            IChannel channel,
            IPayloadChannel payloadChannel,
            Dictionary<string, object>? appData,
            Func<RtpCapabilities> getRouterRtpCapabilities,
            Func<string, Task<Producer?>> getProducerById,
            Func<string, Task<DataProducer?>> getDataProducerById,
            string iceRole,
            IceParameters iceParameters,
            IceCandidate[] iceCandidates,
            IceState iceState,
            TransportTuple? iceSelectedTuple,
            DtlsParameters dtlsParameters,
            DtlsState dtlsState,
            string? dtlsRemoteCert) : base(loggerFactory, transportInternalData, sctpParameters, sctpState, channel, payloadChannel, appData, getRouterRtpCapabilities, getProducerById, getDataProducerById)
        {
            _logger = loggerFactory.CreateLogger<WebRtcTransport>();

            // Data
            IceRole = iceRole;
            IceParameters = iceParameters;
            IceCandidates = iceCandidates;
            IceState = iceState;
            IceSelectedTuple = iceSelectedTuple;
            DtlsParameters = dtlsParameters;
            DtlsState = dtlsState;
            DtlsRemoteCert = dtlsRemoteCert;

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the WebRtcTransport.
        /// </summary>
        public override async Task CloseAsync()
        {
            await CloseLock.WaitAsync();
            try
            {
                if (Closed)
                {
                    return;
                }

                IceState = IceState.Closed;
                IceSelectedTuple = null;
                DtlsState = DtlsState.Closed;

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
            await CloseLock.WaitAsync();
            try
            {
                if (Closed)
                {
                    return;
                }

                IceState = IceState.Closed;
                IceSelectedTuple = null;
                DtlsState = DtlsState.Closed;

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
        /// Provide the WebRtcTransport remote parameters.
        /// </summary>
        public override Task ConnectAsync(object parameters)
        {
            _logger.LogDebug($"ConnectAsync() | WebRtcTransport:{TransportId}");

            return parameters is DtlsParameters dtlsParameters
                ? ConnectAsync(dtlsParameters)
                : throw new Exception($"{nameof(parameters)} type is not DtlsParameters");
        }

        private async Task ConnectAsync(DtlsParameters dtlsParameters)
        {
            var reqData = new { DtlsParameters = dtlsParameters };
            var resData = await Channel.RequestAsync(MethodId.TRANSPORT_CONNECT, Internal, reqData);
            if (resData == null)
            {
                throw new Exception($"{nameof(resData)} is null");
            }
            var responseData = JsonSerializer.Deserialize<WebRtcTransportConnectResponseData>(resData, ObjectExtensions.DefaultJsonSerializerOptions)!;

            // Update data.
            DtlsParameters.Role = responseData.DtlsLocalRole;
        }

        /// <summary>
        /// Restart ICE.
        /// </summary>
        public async Task<IceParameters> RestartIceAsync()
        {
            _logger.LogDebug($"RestartIceAsync() | WebRtcTransport:{TransportId}");

            var resData = await Channel.RequestAsync(MethodId.TRANSPORT_RESTART_ICE, Internal);
            if (resData == null)
            {
                throw new Exception($"{nameof(resData)} is null");
            }
            var responseData = JsonSerializer.Deserialize<WebRtcTransportRestartIceResponseData>(resData, ObjectExtensions.DefaultJsonSerializerOptions)!;

            // Update data.
            IceParameters = responseData.IceParameters;

            return IceParameters;
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
                case "icestatechange":
                    {
                        if (data == null)
                        {
                            _logger.LogWarning($"icestatechange event's data is null.");
                            break;
                        }

                        var notification = JsonSerializer.Deserialize<TransportIceStateChangeNotificationData>(data, ObjectExtensions.DefaultJsonSerializerOptions)!;
                        IceState = notification.IceState;

                        Emit("icestatechange", IceState);

                        // Emit observer event.
                        Observer.Emit("icestatechange", IceState);

                        break;
                    }

                case "iceselectedtuplechange":
                    {
                        if (data == null)
                        {
                            _logger.LogWarning($"iceselectedtuplechange event's data is null.");
                            break;
                        }

                        var notification = JsonSerializer.Deserialize<TransportIceSelectedTupleChangeNotificationData>(data, ObjectExtensions.DefaultJsonSerializerOptions)!;
                        IceSelectedTuple = notification.IceSelectedTuple;

                        Emit("iceselectedtuplechange", IceSelectedTuple);

                        // Emit observer event.
                        Observer.Emit("iceselectedtuplechange", IceSelectedTuple);

                        break;
                    }

                case "dtlsstatechange":
                    {
                        if (data == null)
                        {
                            _logger.LogWarning($"dtlsstatechange event's data is null.");
                            break;
                        }

                        var notification = JsonSerializer.Deserialize<TransportDtlsStateChangeNotificationData>(data, ObjectExtensions.DefaultJsonSerializerOptions)!;
                        DtlsState = notification.DtlsState;

                        if (DtlsState == DtlsState.Connecting)
                        {
                            DtlsRemoteCert = notification.DtlsRemoteCert;
                        }

                        Emit("dtlsstatechange", DtlsState);

                        // Emit observer event.
                        Observer.Emit("dtlsstatechange", DtlsState);

                        break;
                    }

                case "sctpstatechange":
                    {
                        if (data == null)
                        {
                            _logger.LogWarning($"sctpstatechange event's data is null.");
                            break;
                        }

                        var notification = JsonSerializer.Deserialize<TransportSctpStateChangeNotificationData>(data, ObjectExtensions.DefaultJsonSerializerOptions)!;
                        SctpState = notification.SctpState;

                        Emit("sctpstatechange", SctpState);

                        // Emit observer event.
                        Observer.Emit("sctpstatechange", SctpState);

                        break;
                    }

                case "trace":
                    {
                        if (data == null)
                        {
                            _logger.LogWarning($"trace event's data is null.");
                            break;
                        }

                        var trace = JsonSerializer.Deserialize<TransportTraceEventData>(data, ObjectExtensions.DefaultJsonSerializerOptions)!;

                        Emit("trace", trace);

                        // Emit observer event.
                        Observer.Emit("trace", trace);

                        break;
                    }

                default:
                    {
                        _logger.LogError($"OnChannelMessage() | WebRtcTransport:{TransportId} Ignoring unknown event{@event}");
                        break;
                    }
            }
        }

        #endregion Event Handlers
    }
}
