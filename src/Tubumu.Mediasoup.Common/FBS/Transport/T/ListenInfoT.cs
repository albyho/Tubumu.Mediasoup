using System;
using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.Transport
{
    public class ListenInfoT
    {
        public FBS.Transport.Protocol Protocol { get; set; }

        public string Ip { get; set; }

        public string AnnouncedIp { get; set; }

        public ushort Port { get; set; }

        public FBS.Transport.SocketFlagsT Flags { get; set; } = new SocketFlagsT { Ipv6Only = false, UdpReusePort = false };

        public uint SendBufferSize { get; set; }

        public uint RecvBufferSize { get; set; }
    }
}
