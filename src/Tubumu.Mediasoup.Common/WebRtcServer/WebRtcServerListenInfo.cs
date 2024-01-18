using System.Text.Json.Serialization;

namespace Tubumu.Mediasoup
{
    public class WebRtcServerListenInfo
	{
		/// <summary>
		/// Network protocol.
		/// </summary>
		public FBS.Transport.Protocol Protocol { get; set; }

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
	}
}

