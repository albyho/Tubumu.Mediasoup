using FBS.SctpParameters;
using FBS.WebRtcTransport;

namespace Tubumu.Mediasoup
{
    public class CreateWebRtcTransportResult
    {
        public string TransportId { get; set; }

        public IceParametersT IceParameters { get; set; }

        public IceCandidateT[] IceCandidates { get; set; }

        public DtlsParametersT DtlsParameters { get; set; }

        public SctpParameters? SctpParameters { get; set; }
    }
}
