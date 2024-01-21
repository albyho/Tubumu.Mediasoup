using System.Collections.Generic;

namespace FBS.Transport
{
    public class ConsumeRequestT
    {
        public string ConsumerId { get; set; }

        public string ProducerId { get; set; }

        public FBS.RtpParameters.MediaKind Kind { get; set; }

        public FBS.RtpParameters.RtpParametersT RtpParameters { get; set; }

        public FBS.RtpParameters.Type Type { get; set; }

        public List<FBS.RtpParameters.RtpEncodingParametersT> ConsumableRtpEncodings { get; set; }

        public bool Paused { get; set; }

        public FBS.Consumer.ConsumerLayersT? PreferredLayers { get; set; }

        public bool IgnoreDtx { get; set; }
    }
}
