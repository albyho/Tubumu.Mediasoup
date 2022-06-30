namespace Tubumu.Mediasoup
{
    public class DataProducerData
	{
        /// <summary>
        /// SCTP stream parameters.
        /// </summary>
        public SctpStreamParameters? SctpStreamParameters { get; init; }

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

