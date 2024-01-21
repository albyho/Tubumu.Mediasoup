namespace FBS.Producer
{
    public class KeyFrameTraceInfoT
    {
        public FBS.RtpPacket.DumpT RtpPacket { get; set; }

        public bool IsRtx { get; set; }
    }
}
