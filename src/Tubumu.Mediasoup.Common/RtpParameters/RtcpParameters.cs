using System;
using System.Text.Json.Serialization;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Provides information on RTCP settings within the RTP parameters.
    ///
    /// If no cname is given in a producer's RTP parameters, the mediasoup transport
    /// will choose a random one that will be used into RTCP SDES messages sent to
    /// all its associated consumers.
    ///
    /// mediasoup assumes reducedSize to always be true.
    /// </summary>
    [Serializable]
    public class RtcpParameters
    {
        /// <summary>
        /// The Canonical Name (CNAME) used by RTCP (e.g. in SDES messages).
        /// </summary>
        [JsonPropertyName("cname")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CNAME { get; set; }

        /// <summary>
        /// Whether reduced size RTCP RFC 5506 is configured (if true) or compound RTCP
        /// as specified in RFC 3550 (if false). Default true.
        /// </summary>
        public bool? ReducedSize { get; set; } = true;

        /// <summary>
        /// Whether RTCP-mux is used. Default true.
        /// </summary>
        public bool? Mux { get; set; } = true;
    }
}
