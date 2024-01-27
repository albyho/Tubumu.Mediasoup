using FBS.Transport;
using FBS.WebRtcTransport;

namespace Tubumu.Mediasoup
{
    public class WebRtcTransportData : TransportBaseData
    {
        public string IceRole { get; set; }

        public IceParametersT IceParameters { get; set; }

        public IceCandidateT[] IceCandidates { get; set; }

        public IceState IceState { get; set; }

        public TupleT? IceSelectedTuple { get; set; }

        public DtlsParametersT DtlsParameters { get; set; }

        public DtlsState DtlsState { get; set; }

        public string? DtlsRemoteCert { get; set; }
    }
}
