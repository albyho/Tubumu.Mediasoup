using System.Text.Json.Serialization;
using FBS.Transport;

namespace Tubumu.Mediasoup
{
    public class TransportListenInfo
    {
        /// <summary>
        /// Network protocol.
        /// </summary>
        public Protocol Protocol { get; set; }

        /// <summary>
        /// Listening IPv4 or IPv6.
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Announced IPv4 or IPv6 (useful when running mediasoup behind NAT with private IP).
        /// </summary>
        public string? AnnouncedIp { get; set; }

        /// <summary>
        /// Listening port.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ushort? Port { get; set; }

        /// <summary>
        /// Socket flags.
        /// </summary>
        /// <value></value>
        public TransportSocketFlags? Flags { get; set; }

        /// <summary>
        /// Send buffer size (bytes).
        /// </summary>
        /// <value></value>
        public uint? SendBufferSize { get; set; }

        /// <summary>
        /// Recv buffer size (bytes).
        /// </summary>
        /// <value></value>
        public uint? RecvBufferSize { get; set; }
    }
}
