namespace FBS.RtpStream
{
    public class BaseStatsT
    {
        public ulong Timestamp { get; set; }

        public uint Ssrc { get; set; }

        public FBS.RtpParameters.MediaKind Kind { get; set; }

        public string MimeType { get; set; }

        public ulong PacketsLost { get; set; }

        public byte FractionLost { get; set; }

        public ulong PacketsDiscarded { get; set; }

        public ulong PacketsRetransmitted { get; set; }

        public ulong PacketsRepaired { get; set; }

        public ulong NackCount { get; set; }

        public ulong NackPacketCount { get; set; }

        public ulong PliCount { get; set; }

        public ulong FirCount { get; set; }

        public byte Score { get; set; }

        public string Rid { get; set; }

        public uint? RtxSsrc { get; set; }

        public ulong RtxPacketsDiscarded { get; set; }

        public float RoundTripTime { get; set; }
    }
}
