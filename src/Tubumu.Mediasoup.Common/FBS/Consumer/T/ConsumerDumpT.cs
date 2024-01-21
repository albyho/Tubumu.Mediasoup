using System.Collections.Generic;

namespace FBS.Consumer
{
    public class ConsumerDumpT
    {
        public FBS.Consumer.BaseConsumerDumpT Base { get; set; }

        public List<FBS.RtpStream.DumpT> RtpStreams { get; set; }

        public short? PreferredSpatialLayer { get; set; }

        public short? TargetSpatialLayer { get; set; }

        public short? CurrentSpatialLayer { get; set; }

        public short? PreferredTemporalLayer { get; set; }

        public short? TargetTemporalLayer { get; set; }

        public short? CurrentTemporalLayer { get; set; }
    }
}
