using System.Text.Json.Serialization;

namespace Tubumu.Mediasoup
{
    public class TransportInternal : RouterInternal
    {
        /// <summary>
        /// Trannsport id.
        /// </summary>
        public string TransportId { get; }

        /// <summary>
        /// WebRtcServer id.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? WebRtcServerId { get; }

        public TransportInternal(string routerId, string transportId, string? webRtcServerId = null)
        {
            RouterId = routerId;
            TransportId = transportId;
            WebRtcServerId = webRtcServerId;
        }
    }
}

