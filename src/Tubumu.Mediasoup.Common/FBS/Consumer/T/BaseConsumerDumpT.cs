using System.Collections.Generic;

namespace FBS.Consumer
{
    public class BaseConsumerDumpT
    {
        public string Id { get; set; }

        public FBS.RtpParameters.Type Type { get; set; }

        public string ProducerId { get; set; }

        public FBS.RtpParameters.MediaKind Kind { get; set; }

        public FBS.RtpParameters.RtpParametersT RtpParameters { get; set; }

        public List<FBS.RtpParameters.RtpEncodingParametersT> ConsumableRtpEncodings { get; set; }

        public List<byte> SupportedCodecPayloadTypes { get; set; }

        public List<FBS.Consumer.TraceEventType> TraceEventTypes { get; set; }

        public bool Paused { get; set; }

        public bool ProducerPaused { get; set; }

        public byte Priority { get; set; }
    }
}
