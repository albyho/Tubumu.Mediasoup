namespace Tubumu.Mediasoup
{
    public class PlainTransportStat : TransportStat
    {
        // PlainTransport specific.
        public bool RtcpMux { get; set; }

        public bool Comedia { get; set; }

        public TransportTuple Tuple { get; set; }

        public TransportTuple? RtcpTuple { get; set; }
    }
}
