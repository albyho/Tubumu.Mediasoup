using System.Runtime.Serialization;
using Tubumu.Utils.Extensions;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Valid types for 'trace' event.
    /// </summary>
    public enum TraceEventType
    {
        /// <summary>
        /// RTP
        /// </summary>
        [EnumMember(Value = "rtp")]
        RTP,

        /// <summary>
        /// 关键帧
        /// </summary>
        [EnumMember(Value = "keyframe")]
        Keyframe,

        /// <summary>
        /// NACK
        /// </summary>
        [EnumMember(Value = "nack")]
        nack,

        /// <summary>
        /// PLI: (Picture Loss Indication) 视频帧丢失重传
        /// </summary>
        [EnumMember(Value = "pli")]
        PLI,

        /// <summary>
        /// Full Intra Request
        /// </summary>
        [EnumMember(Value = "fir")]
        FIR
    }
}
