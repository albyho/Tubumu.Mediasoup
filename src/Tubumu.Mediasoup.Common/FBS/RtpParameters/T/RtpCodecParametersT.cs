using System.Collections.Generic;

namespace FBS.RtpParameters
{
    public class RtpCodecParametersT
    {
        public string MimeType { get; set; }

        public byte PayloadType { get; set; }

        public uint ClockRate { get; set; }

        public byte? Channels { get; set; }

        public List<ParameterT> Parameters { get; set; }

        public List<RtcpFeedbackT> RtcpFeedback { get; set; }
    }
}
