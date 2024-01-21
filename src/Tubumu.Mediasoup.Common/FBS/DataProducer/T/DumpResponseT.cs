namespace FBS.DataProducer
{
    public class DumpResponseT
    {
        public string Id { get; set; }

        public FBS.DataProducer.Type Type { get; set; }

        public FBS.SctpParameters.SctpStreamParametersT SctpStreamParameters { get; set; }

        public string Label { get; set; }

        public string Protocol { get; set; }

        public bool Paused { get; set; }
    }
}
