namespace FBS.RtpStream
{
    public class DumpT
    {
        public FBS.RtpStream.ParamsT Params { get; set; }

        public byte Score { get; set; }

        public FBS.RtxStream.RtxDumpT RtxStream { get; set; }
    }
}
