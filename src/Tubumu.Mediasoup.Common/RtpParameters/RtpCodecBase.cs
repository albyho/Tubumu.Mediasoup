using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Tubumu.Mediasoup
{
    [Serializable]
    public class RtpCodecBase
    {
        /// <summary>
        /// The codec MIME media type/subtype (e.g. 'audio/opus', 'video/VP8').
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Codec clock rate expressed in Hertz.
        /// </summary>
        public int ClockRate { get; set; }

        /// <summary>
        /// The number of channels supported (e.g. two for stereo). Just for audio.
        /// Default 1.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Channels { get; set; }

        /// <summary>
        /// Codec-specific parameters available for signaling. Some parameters (such
        /// as 'packetization-mode' and 'profile-level-id' in H264 or 'profile-id' in
        /// VP9) are critical for codec matching.
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }
    }
}
