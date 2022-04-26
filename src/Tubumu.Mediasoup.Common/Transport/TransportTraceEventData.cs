using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// 'trace' event data.
    /// </summary>
    public class TransportTraceEventData
    {
        /// <summary>
        /// Trace type.
        /// </summary>
        public TransportTraceEventType Type { get; set; }

        /// <summary>
        /// Event timestamp.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Event direction.
        /// </summary>
        public TraceEventDirection Direction { get; set; }

        /// <summary>
        /// Per type information.
        /// </summary>
        public Dictionary<string, object> Info { get; set; }
    }
}
