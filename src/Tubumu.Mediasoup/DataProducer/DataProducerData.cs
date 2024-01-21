using FBS.SctpParameters;

namespace Tubumu.Mediasoup
{
    public class DataProducerData
    {
        public FBS.DataProducer.Type Type { get; set; }

        /// <summary>
        /// SCTP stream parameters.
        /// </summary>
        public SctpStreamParametersT? SctpStreamParameters { get; init; }

        /// <summary>
        /// DataChannel label.
        /// </summary>
        public string Label { get; init; }

        /// <summary>
        /// DataChannel protocol.
        /// </summary>
        public string Protocol { get; init; }
    }
}

