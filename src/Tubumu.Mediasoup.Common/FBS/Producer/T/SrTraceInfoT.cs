namespace FBS.Producer
{
    public class SrTraceInfoT
    {
        public uint Ssrc { get; set; }

        public uint NtpSec { get; set; }

        public uint NtpFrac { get; set; }

        public uint RtpTs { get; set; }

        public uint PacketCount { get; set; }

        public uint OctetCount { get; set; }
    }
}
