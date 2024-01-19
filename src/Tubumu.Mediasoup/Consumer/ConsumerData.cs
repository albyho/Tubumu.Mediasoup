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
        public Type Type { get; }

        public ConsumerData(string producerId, MediaKind kind, RtpParametersT rtpParameters, Type type)
        {
            ProducerId = producerId;
            Kind = kind;
            RtpParameters = rtpParameters;
            Type = type;
        }
    }
}

