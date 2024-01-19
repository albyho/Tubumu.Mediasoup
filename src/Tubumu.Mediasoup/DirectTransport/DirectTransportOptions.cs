using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    public class DirectTransportOptions
    {
        /// <summary>
        /// Maximum allowed size for direct messages sent from DataProducers.
        /// Default 262144.
        /// </summary>
        public uint MaxMessageSize { get; set; } = 262144;

        /// <summary>
        /// Custom application data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; set; }
    }
}
