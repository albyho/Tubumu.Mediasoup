using FBS.SctpParameters;

namespace Tubumu.Mediasoup
{
    public class TransportBaseData
    {
        /// <summary>
        /// SCTP parameters.
        /// </summary>
        public SctpParametersT? SctpParameters { get; set; }

        /// <summary>
        /// Sctp state.
        /// </summary>
        public SctpState? SctpState { get; set; }
    }
}

