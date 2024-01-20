using System.Collections.Generic;

namespace FBS.RtpParameters
{
    public class RtpParametersT
    {
        /// <summary>
        /// Mid. Nullable.
        /// </summary>
        /// <value></value>
        public string? Mid { get; set; }

        public List<RtpCodecParametersT> Codecs { get; set; }


        public List<RtpHeaderExtensionParametersT> HeaderExtensions { get; set; }

        public List<RtpEncodingParametersT> Encodings { get; set; }

        public RtcpParametersT Rtcp { get; set; }
    }
}
