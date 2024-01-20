using System.Text.Json.Serialization;

namespace FBS.RtpParameters
{
    /// <summary>
    /// <para>Provides information on RTCP settings within the RTP parameters.</para>
    /// <para>
    /// If no cname is given in a producer's RTP parameters, the mediasoup transport
    /// will choose a random one that will be used into RTCP SDES messages sent to
    /// all its associated consumers.
    /// </para>
    /// <para>mediasoup assumes reducedSize to always be true.</para>
    /// </summary>
    public class RtcpParametersT
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
        /// <value></value>
        public bool ReducedSize { get; set; } = true;
    }
}
