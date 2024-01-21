namespace FBS.PlainTransport
{
    public class GetStatsResponseT
    {
        public FBS.Transport.StatsT Base { get; set; }

        public bool RtcpMux { get; set; }

        public bool Comedia { get; set; }

        public FBS.Transport.TupleT Tuple { get; set; }

        public FBS.Transport.TupleT RtcpTuple { get; set; }
    }
}
