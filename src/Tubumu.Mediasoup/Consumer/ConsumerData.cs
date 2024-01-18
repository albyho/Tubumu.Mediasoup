using FBS.RtpParameters;

namespace Tubumu.Mediasoup
{
    public class ConsumerData
    {
        /// <summary>
        /// Associated Producer id.
        /// </summary>
        public string ProducerId { get; }

        /// <summary>
        /// Media kind.
        /// </summary>
        public MediaKind Kind { get; }

        /// <summary>
        /// RTP parameters.
        /// </summary>
        public RtpParametersT RtpParameters { get; }

        /// <summary>
        /// Consumer type.
        /// </summary>
        public ConsumerType Type { get; }

        public ConsumerData(string producerId, MediaKind kind, RtpParameters rtpParameters, ConsumerType type)
        {
            ProducerId = producerId;
            Kind = kind;
            RtpParameters = rtpParameters;
            Type = type;
        }
    }
}

