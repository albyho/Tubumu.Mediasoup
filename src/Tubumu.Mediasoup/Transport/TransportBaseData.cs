namespace Tubumu.Mediasoup
{
    public class TransportBaseData
    {
        /// <summary>
        /// SCTP parameters.
        /// </summary>
        public SctpParameters? SctpParameters { get; set; }

        /// <summary>
        /// Sctp state.
        /// </summary>
        public SctpState? SctpState { get; set; }
    }
}

