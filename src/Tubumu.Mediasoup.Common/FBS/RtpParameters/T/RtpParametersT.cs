using System.Collections.Generic;

namespace FBS.RtpParameters
{
    public class RtpParametersT
    {
        public string Mid { get; set; }

        public List<FBS.RtpParameters.RtpCodecParametersT> Codecs { get; set; }


        public List<FBS.RtpParameters.RtpHeaderExtensionParametersT> HeaderExtensions { get; set; }

        public List<FBS.RtpParameters.RtpEncodingParametersT> Encodings { get; set; }

        public FBS.RtpParameters.RtcpParametersT Rtcp { get; set; }
    }
}
