using System;
using System.Text.Json.Serialization;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Provides information relating to an encoding, which represents a media RTP
    /// stream and its associated RTX stream (if any).
    /// </summary>
    [Serializable]
    public class RtpEncodingParameters
    {
        /// <summary>
        /// The media SSRC.
        /// </summary>
        public uint Ssrc { get; set; }

        /// <summary>
        /// The RID RTP extension value. Must be unique.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // FIX: libmediasoupclient "invalid encoding.rid" error.
        public string? Rid { get; set; }

        /// <summary>
        /// Codec payload type this encoding affects. If unset, first media codec is
        /// chosen.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CodecPayloadType { get; set; }

        /// <summary>
        /// RTX stream information. It must contain a numeric ssrc field indicating
        /// the RTX SSRC.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // FIX: libmediasoupclient "invalid encoding.rtx" error.
        public Rtx? Rtx { get; set; }

        /// <summary>
        /// It indicates whether discontinuous RTP transmission will be used. Useful
        /// for audio (if the codec supports it) and for video screen sharing (when
        /// static content is being transmitted, this option disables the RTP
        /// inactivity checks in mediasoup). Default false.
        /// </summary>
        public bool? Dtx { get; set; } = false;

        /// <summary>
        /// Number of spatial and temporal layers in the RTP stream (e.g. 'L1T3').
        /// See webrtc-svc.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // FIX: libmediasoupclient "invalid encoding.scalabilityMode" error.
        public string? ScalabilityMode { get; set; }

        /// <summary>
        /// Others.
        /// </summary>
        public int? ScaleResolutionDownBy;

        public int? MaxBitrate;
    }
}
