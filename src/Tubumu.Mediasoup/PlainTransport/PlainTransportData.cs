namespace Tubumu.Mediasoup
{
    public class PlainTransportData : TransportBaseData
	{
        public bool? RtcpMux { get; set; }

        public bool? Comedia { get; set; }

        public TransportTuple Tuple { get; set; }

        public TransportTuple? RtcpTuple { get; set; }

        public SrtpParameters? SrtpParameters { get; set; }
    }
}
