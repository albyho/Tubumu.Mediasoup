namespace Tubumu.Mediasoup
{
    public class SctpParameters
    {
        /// <summary>
        /// Must always equal 5000.
        /// </summary>
        public int Port { get; set; } = 5000;

        /// <summary>
        /// Initially requested number of outgoing SCTP streams.
        /// </summary>
        public int OS { get; set; }

        /// <summary>
        /// Maximum number of incoming SCTP streams.
        /// </summary>
        public int MIS { get; set; }

        /// <summary>
        /// Maximum allowed size for SCTP messages.
        /// </summary>
        public int MaxMessageSize { get; set; }
    }
}
