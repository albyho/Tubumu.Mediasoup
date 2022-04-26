using System;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Provides information on RTCP feedback messages for a specific codec. Those
    /// messages can be transport layer feedback messages or codec-specific feedback
    /// messages. The list of RTCP feedbacks supported by mediasoup is defined in the
    /// supportedRtpCapabilities.ts file.
    /// </summary>
    [Serializable]
    public class RtcpFeedback
    {
        /// <summary>
        /// RTCP feedback type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// RTCP feedback parameter.
        /// </summary>
        public string? Parameter { get; set; }
    }
}
