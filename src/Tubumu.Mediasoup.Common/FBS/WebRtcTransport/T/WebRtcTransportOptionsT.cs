namespace FBS.WebRtcTransport
{
    public class WebRtcTransportOptionsT
    {
        public FBS.Transport.OptionsT Base { get; set; }

        public FBS.WebRtcTransport.ListenUnion Listen { get; set; }

        public bool EnableUdp { get; set; }

        public bool EnableTcp { get; set; }

        public bool PreferUdp { get; set; }

        public bool PreferTcp { get; set; }
    }
}
