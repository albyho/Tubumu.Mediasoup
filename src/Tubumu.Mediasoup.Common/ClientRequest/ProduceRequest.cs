using System.Collections.Generic;
using FBS.RtpParameters;

namespace Tubumu.Mediasoup
{
    public class ProduceRequest
    {
        public MediaKind Kind { get; set; }

        public RtpParametersT RtpParameters { get; set; }

        public string Source { get; set; }

        public Dictionary<string, object> AppData { get; set; }
    }
}
