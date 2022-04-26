using System;
using Tubumu.Utils.Extensions;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Provides information on codec settings within the RTP parameters. The list
    /// of media codecs supported by mediasoup and their settings is defined in the
    /// supportedRtpCapabilities.ts file.
    /// </summary>
    [Serializable]
    public class RtpCodecParameters : RtpCodecBase, IEquatable<RtpCodecParameters>
    {
        /// <summary>
        /// The value that goes in the RTP Payload Type Field. Must be unique.
        /// </summary>
        public int PayloadType { get; set; }

        /// <summary>
        /// Transport layer and codec-specific feedback messages for this codec.
        /// </summary>
        public RtcpFeedback[]? RtcpFeedback { get; set; }

        public bool Equals(RtpCodecParameters? other)
        {
            if (other == null)
            {
                return false;
            }

            var result = (MimeType == other.MimeType)
                && (PayloadType == other.PayloadType)
                && (ClockRate == other.ClockRate);
            if (result)
            {
                if (Channels.HasValue && other.Channels.HasValue)
                {
                    result = Channels == other.Channels;
                }
                else if ((Channels.HasValue && !other.Channels.HasValue) || (!Channels.HasValue && other.Channels.HasValue))
                {
                    result = false;
                }
            }
            if (result)
            {
                if (Parameters != null && other.Parameters != null)
                {
                    result = Parameters.DeepEquals(other.Parameters);
                }
                else if ((Parameters == null && other.Parameters != null) || (Parameters != null && other.Parameters == null))
                {
                    result = false;
                }
            }

            return result;
        }

        public override bool Equals(object? other)
        {
            if (other is RtpCodecParameters)
            {
                return Equals((RtpCodecParameters)other);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var result = MimeType.GetHashCode() ^ PayloadType.GetHashCode() ^ ClockRate.GetHashCode();
            if (Parameters != null)
            {
                result = Parameters.DeepGetHashCode() ^ result;
            }
            return result;
        }
    }
}
