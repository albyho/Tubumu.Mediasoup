namespace Tubumu.Mediasoup
{
    public class RouterCreateWebRtcTransportResponseData
    {
        public string IceRole { get; set; }

        public IceParameters IceParameters { get; set; }

        public IceCandidate[] IceCandidates { get; set; }

        public IceState IceState { get; set; }

        public TransportTuple? IceSelectedTuple { get; set; }

        public DtlsParameters DtlsParameters { get; set; }

        public DtlsState DtlsState { get; set; }

        public string? DtlsRemoteCert { get; set; }
    }

    public class RouterCreatePlainTransportResponseData
    {
        public bool? RtcpMux { get; set; }

        public bool? Comedia { get; set; }

        public TransportTuple Tuple { get; set; }

        public TransportTuple? RtcpTuple { get; set; }

        public SrtpParameters? SrtpParameters { get; set; }
    }

    public class RouterCreatePipeTransportResponseData
    {
        public TransportTuple Tuple { get; set; }

        public bool Rtx { get; set; }

        public SrtpParameters? SrtpParameters { get; set; }
    }

    public class TransportProduceResponseData
    {
        public ProducerType Type { get; set; }
    }

    public class TransportConsumeResponseData
    {
        public bool Paused { get; set; }

        public bool ProducerPaused { get; set; }

        public ConsumerScore Score { get; set; }

        public ConsumerLayers PreferredLayers { get; set; }
    }

    public class TransportDataProduceResponseData
    {
        /// <summary>
        /// DataProducer id (just for Router.pipeToRouter() method).
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// SCTP parameters defining how the endpoint is sending the data.
        /// </summary>
        public SctpStreamParameters SctpStreamParameters { get; set; }

        /// <summary>
        /// A label which can be used to distinguish this DataChannel from others.
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// Name of the sub-protocol used by this DataChannel.
        /// </summary>
        public string? Protocol { get; set; }
    }

    public class TransportDataConsumeResponseData
    {
        /// <summary>
        /// DataConsumer id (just for Router.pipeToRouter() method).
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// SCTP parameters defining how the endpoint is sending the data.
        /// </summary>
        public SctpStreamParameters SctpStreamParameters { get; set; }

        /// <summary>
        /// A label which can be used to distinguish this DataChannel from others.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Name of the sub-protocol used by this DataChannel.
        /// </summary>
        public string Protocol { get; set; }
    }

    public class ConsumerSetPreferredLayersResponseData : ConsumerLayers
    {
    }

    public class ConsumerSetOrUnsetPriorityResponseData
    {
        public int Priority { get; set; }
    }

    public class WebRtcTransportConnectResponseData
    {
        public DtlsRole? DtlsLocalRole { get; set; }
    }

    public class PipeTransportConnectResponseData
    {
        public TransportTuple Tuple { get; set; }

        public SrtpParameters? SrtpParameters { get; set; }
    }

    public class PlainTransportConnectResponseData
    {
        public TransportTuple Tuple { get; set; }

        public TransportTuple? RtcpTuple { get; set; }

        public SrtpParameters? SrtpParameters { get; set; }
    }

    public class WebRtcTransportRestartIceResponseData
    {
        public IceParameters IceParameters { get; set; }
    }
}
