using System.Collections.Generic;

namespace FBS.DataConsumer
{
    public class DumpResponseT
    {
        public string Id { get; set; }

        public string DataProducerId { get; set; }

        public FBS.DataProducer.Type Type { get; set; }

        public FBS.SctpParameters.SctpStreamParametersT SctpStreamParameters { get; set; }

        public string Label { get; set; }

        public string Protocol { get; set; }

        public uint BufferedAmountLowThreshold { get; set; }

        public bool Paused { get; set; }

        public bool DataProducerPaused { get; set; }

        public List<ushort> Subchannels { get; set; }
    }
}
