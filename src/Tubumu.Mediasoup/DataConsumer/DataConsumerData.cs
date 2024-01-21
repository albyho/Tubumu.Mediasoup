using FBS.SctpParameters;

namespace Tubumu.Mediasoup
{
    public class DataConsumerData
    {
        /// <summary>
        /// Associated DataProducer id.
        /// </summary>
        public string DataProducerId { get; init; }

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

        public uint BufferedAmountLowThreshold { get; set; }
    }
}
