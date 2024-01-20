namespace FBS.PlainTransport
{
    public class PlainTransportOptionsT
    {
        public FBS.Transport.OptionsT Base { get; set; }

        public FBS.Transport.ListenInfoT ListenInfo { get; set; }

        /// <summary>
        /// RtcpListenInfo. Nullable.
        /// </summary>
        /// <value></value>
        public FBS.Transport.ListenInfoT? RtcpListenInfo { get; set; }

        public bool RtcpMux { get; set; }

        public bool Comedia { get; set; }

        public bool EnableSrtp { get; set; }

        public FBS.SrtpParameters.SrtpCryptoSuite? SrtpCryptoSuite { get; set; }
    }
}
