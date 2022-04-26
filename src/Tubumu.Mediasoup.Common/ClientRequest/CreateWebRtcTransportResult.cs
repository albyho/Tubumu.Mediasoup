using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Tubumu.Mediasoup
{
    public class CreateWebRtcTransportResult
    {
        public string TransportId { get; set; }

        public IceParameters IceParameters { get; set; }

        public IceCandidate[] IceCandidates { get; set; }

        public DtlsParameters DtlsParameters { get; set; }

        public SctpParameters? SctpParameters { get; set; }
    }
}
