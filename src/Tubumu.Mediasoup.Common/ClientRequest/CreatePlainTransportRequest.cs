namespace Tubumu.Mediasoup
{
    public class CreatePlainTransportRequest
    {
        /// <summary>
        /// Whether remote IP:port should be auto-detected based on first RTP/RTCP packet received.
        /// If enabled, connect() must only be called if SRTP is enabled by providing the remote srtpParameters and nothing else.
        /// </summary>
        public bool Comedia { get; set; }

        /// <summary>
        /// Use RTCP-mux (RTP and RTCP in the same port).
        /// </summary>
        public bool RtcpMux { get; set; } = true;

        public bool Producing { get; set; }

        public bool Consuming { get; set; }
    }
}
