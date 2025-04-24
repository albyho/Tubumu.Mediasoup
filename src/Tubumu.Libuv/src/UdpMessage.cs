using System;
using System.Net;

namespace Tubumu.Libuv
{
    public class UdpMessage : IMessage<IPEndPoint, ArraySegment<byte>>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public UdpMessage()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        { }

        public UdpMessage(IPEndPoint endPoint, byte[] payload)
            : this(endPoint, new ArraySegment<byte>(payload)) { }

        public UdpMessage(IPEndPoint endPoint, byte[] payload, int offset, int count)
            : this(endPoint, new ArraySegment<byte>(payload, offset, count)) { }

        public UdpMessage(IPAddress ipAddress, int port, byte[] payload)
            : this(new IPEndPoint(ipAddress, port), payload) { }

        public UdpMessage(IPAddress ipAddress, int port, byte[] payload, int offset, int count)
            : this(new IPEndPoint(ipAddress, port), new ArraySegment<byte>(payload, offset, count)) { }

        public UdpMessage(IPAddress ipAddress, int port, ArraySegment<byte> payload)
            : this(new IPEndPoint(ipAddress, port), payload) { }

        public UdpMessage(string ipAddress, int port, byte[] payload)
            : this(IPAddress.Parse(ipAddress), port, payload) { }

        public UdpMessage(string ipAddress, int port, ArraySegment<byte> payload)
            : this(IPAddress.Parse(ipAddress), port, payload) { }

        public UdpMessage(IPEndPoint endPoint, ArraySegment<byte> payload)
        {
            EndPoint = endPoint;
            Payload = payload;
        }

        public IPEndPoint EndPoint { get; set; }
        public ArraySegment<byte> Payload { get; set; }
    }
}
