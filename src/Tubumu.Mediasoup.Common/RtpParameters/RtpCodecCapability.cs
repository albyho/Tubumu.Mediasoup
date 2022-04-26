using System;
using System.Text.Json.Serialization;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Provides information on the capabilities of a codec within the RTP
    /// capabilities. The list of media codecs supported by mediasoup and their
    /// settings is defined in the supportedRtpCapabilities.ts file.
    ///
    /// Exactly one RtpCodecCapability will be present for each supported combination
    /// of parameters that requires a distinct value of preferredPayloadType. For
    /// example:
    ///
    /// - Multiple H264 codecs, each with their own distinct 'packetization-mode' and
    /// 'profile-level-id' values.
    /// - Multiple VP9 codecs, each with their own distinct 'profile-id' value.
    ///
    /// RtpCodecCapability entries in the mediaCodecs array of RouterOptions do not
    /// require preferredPayloadType field (if unset, mediasoup will choose a random
    /// one). If given, make sure it's in the 96-127 range.
    /// </summary>
    [Serializable]
    public class RtpCodecCapability : RtpCodecBase
    {
        /// <summary>
        /// Media kind.
        /// </summary>
        public MediaKind Kind { get; set; }

        /// <summary>
        /// The preferred RTP payload type.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PreferredPayloadType { get; set; }

        /// <summary>
        /// Transport layer and codec-specific feedback messages for this codec.
        /// </summary>
        public RtcpFeedback[]? RtcpFeedback { get; set; }
    }
}
