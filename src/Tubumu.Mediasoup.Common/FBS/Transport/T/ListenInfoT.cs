using System.ComponentModel.DataAnnotations;

namespace FBS.Transport
{
    public class ListenInfoT
    {
        public Protocol Protocol { get; set; }

        [Required]
        public string Ip { get; set; }

        public string? AnnouncedAddress { get; set; }

        public ushort Port { get; set; }

        [Required]
        public SocketFlagsT Flags { get; set; } = new SocketFlagsT { Ipv6Only = false, UdpReusePort = false };

        public uint SendBufferSize { get; set; }

        public uint RecvBufferSize { get; set; }
    }
}
