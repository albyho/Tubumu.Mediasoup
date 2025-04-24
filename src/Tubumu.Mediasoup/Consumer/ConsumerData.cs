using FBS.RtpParameters;

namespace Tubumu.Mediasoup
{
    public class ConsumerData
    {
        /// <summary>
        /// Associated Producer id.
        /// </summary>
        public string ProducerId { get; init; }

        /// <summary>
        /// Media kind.
        /// </summary>
        public MediaKind Kind { get; init; }

        /// <summary>
        /// RTP parameters.
        /// </summary>
        public RtpParameters RtpParameters { get; init; }

        /// <summary>
        /// Consumer type.
        /// </summary>
        public Type Type { get; init; }
    }
}
