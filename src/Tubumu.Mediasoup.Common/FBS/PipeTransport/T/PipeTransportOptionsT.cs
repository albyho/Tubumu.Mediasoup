namespace FBS.PipeTransport
{
    public class PipeTransportOptionsT
    {
        public FBS.Transport.OptionsT Base { get; set; }

        public FBS.Transport.ListenInfoT ListenInfo { get; set; }

        public bool EnableRtx { get; set; }

        public bool EnableSrtp { get; set; }
    }
}
