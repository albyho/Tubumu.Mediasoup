using System.Collections.Generic;

namespace FBS.Transport
{
    public class ConsumeDataRequestT
    {
        public string DataConsumerId { get; set; }

        public string DataProducerId { get; set; }

        public FBS.DataProducer.Type Type { get; set; }

        public FBS.SctpParameters.SctpStreamParametersT? SctpStreamParameters { get; set; }

        public string Label { get; set; }

        public string Protocol { get; set; }

        public bool Paused { get; set; }

        public List<ushort>? Subchannels { get; set; }
    }
}
