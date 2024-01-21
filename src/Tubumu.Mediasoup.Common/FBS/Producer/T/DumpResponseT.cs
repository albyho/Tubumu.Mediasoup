using System.Collections.Generic;

namespace FBS.Producer
{
    public class DumpResponseT
    {
        public string Id { get; set; }

        public FBS.RtpParameters.MediaKind Kind { get; set; }

        public FBS.RtpParameters.Type Type { get; set; }

        public FBS.RtpParameters.RtpParametersT RtpParameters { get; set; }

        public FBS.RtpParameters.RtpMappingT RtpMapping { get; set; }

        public List<FBS.RtpStream.DumpT> RtpStreams { get; set; }

        public List<FBS.Producer.TraceEventType> TraceEventTypes { get; set; }

        public bool Paused { get; set; }
    }
}
