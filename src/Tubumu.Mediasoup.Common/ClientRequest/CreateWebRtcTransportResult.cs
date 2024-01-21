using System.Collections.Generic;
using FBS.SctpParameters;
using FBS.WebRtcTransport;

namespace Tubumu.Mediasoup
{
    public class CreateWebRtcTransportResult
    {
        public string TransportId { get; set; }

        public IceParametersT IceParameters { get; set; }

        public List<IceCandidateT> IceCandidates { get; set; }

        public DtlsParametersT DtlsParameters { get; set; }

        public SctpParametersT? SctpParameters { get; set; }
    }
}
