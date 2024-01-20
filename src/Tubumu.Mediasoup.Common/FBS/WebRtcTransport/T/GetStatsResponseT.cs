namespace FBS.WebRtcTransport
{
    public class GetStatsResponseT
    {
        public FBS.Transport.StatsT Base { get; set; }

        public FBS.WebRtcTransport.IceRole IceRole { get; set; }

        public FBS.WebRtcTransport.IceState IceState { get; set; }

        public FBS.Transport.TupleT IceSelectedTuple { get; set; }

        public FBS.WebRtcTransport.DtlsState DtlsState { get; set; }
    }
}
