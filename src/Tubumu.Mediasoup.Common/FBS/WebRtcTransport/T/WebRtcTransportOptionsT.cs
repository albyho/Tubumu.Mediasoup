namespace FBS.WebRtcTransport
{
    public class WebRtcTransportOptionsT
    {
        public FBS.Transport.OptionsT Base { get; set; }

        public ListenUnion Listen { get; set; }

        public bool EnableUdp { get; set; } = true;

        public bool EnableTcp { get; set; } = true;

        public bool PreferUdp { get; set; }

        public bool PreferTcp { get; set; }

        public byte IceConsentTimeout { get; set; } = 30;
    }
}
