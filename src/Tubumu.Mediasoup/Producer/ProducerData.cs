namespace Tubumu.Mediasoup
{
    public class ProducerData
	{
        /// <summary>
        /// Media kind.
        /// </summary>
        public MediaKind Kind { get; init; }

        /// <summary>
        /// RTP parameters.
        /// </summary>
        public RtpParameters RtpParameters { get; init; }

        /// <summary>
        /// Producer type.
        /// </summary>
        public ProducerType Type { get; init; }

        /// <summary>
        /// Consumable RTP parameters.
        /// </summary>
        public RtpParameters ConsumableRtpParameters { get; init; }
    }
}

