namespace Tubumu.Mediasoup
{
    public class WebRtcTransportData : TransportBaseData
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
}

