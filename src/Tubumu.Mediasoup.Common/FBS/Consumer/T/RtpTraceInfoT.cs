namespace FBS.Consumer
{
    public class RtpTraceInfoT
    {
        public FBS.RtpPacket.DumpT RtpPacket { get; set; }

        public bool IsRtx { get; set; }
    }
}
