using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    public class ProduceRequest
    {
        public FBS.RtpParameters.MediaKind Kind { get; set; }

        public FBS.RtpParameters.RtpParametersT RtpParameters { get; set; }

        public string Source { get; set; }

        public Dictionary<string, object> AppData { get; set; }
    }
}
