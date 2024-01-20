namespace FBS.WebRtcTransport
{
    public class IceCandidateT
    {
        public string Foundation { get; set; }

        public uint Priority { get; set; }

        public string Ip { get; set; }

        public FBS.Transport.Protocol Protocol { get; set; }

        public ushort Port { get; set; }

        public FBS.WebRtcTransport.IceCandidateType Type { get; set; }

        public FBS.WebRtcTransport.IceCandidateTcpType? TcpType { get; set; }
    }
}
