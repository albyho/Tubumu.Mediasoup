namespace Tubumu.Mediasoup
{
    public class WebRtcTransportStat : TransportStat
    {
        // WebRtcTransport specific.
        public string IceRole { get; set; }

        public IceState IceState { get; set; }

        public TransportTuple? IceSelectedTuple { get; set; }

        public DtlsState DtlsState { get; set; }
    }
}
