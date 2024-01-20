using FBS.Consumer;
using FBS.SctpParameters;
using FBS.SrtpParameters;
using FBS.Transport;
using FBS.WebRtcTransport;

namespace Tubumu.Mediasoup
{
    public class RouterCreateWebRtcTransportResponseData : WebRtcTransportData
    {
    }

    public class RouterCreatePlainTransportResponseData : PlainTransportData
    {
    }

    public class RouterCreatePipeTransportResponseData : PipeTransportData
    {
    }

    public class RouterCreateDirectTransportResponseData : PipeTransportData
    {
    }

    public class TransportProduceResponseData
    {
        /// <summary>
        /// Producer donot support `pipe`
        /// </summary>
        public FBS.RtpParameters.Type Type { get; set; }
    }

    public class TransportConsumeResponseData
    {
        public bool Paused { get; set; }

        public bool ProducerPaused { get; set; }

        public ConsumerScoreT Score { get; set; }

        public ConsumerLayersT PreferredLayers { get; set; }
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
        public SctpStreamParametersT SctpStreamParameters { get; set; }

        /// <summary>
        /// A label which can be used to distinguish this DataChannel from others.
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// Name of the sub-protocol used by this DataChannel.
        /// </summary>
        public string? Protocol { get; set; }
    }

    public class TransportDataConsumeResponseData : DataConsumerData
    {
    }

    public class ConsumerSetPreferredLayersResponseData : ConsumerLayersT
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
        public TupleT Tuple { get; set; }

        public SrtpParametersT? SrtpParameters { get; set; }
    }

    public class PlainTransportConnectResponseData
    {
        public TupleT Tuple { get; set; }

        public TupleT? RtcpTuple { get; set; }

        public SrtpParametersT? SrtpParameters { get; set; }
    }

    public class WebRtcTransportRestartIceResponseData
    {
        public IceParametersT IceParameters { get; set; }
    }
}
