using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    public class ConsumerOptionsBase
    {
        /// <summary>
        /// The id of the Producer to consume.
        /// </summary>
        public string ProducerId { get; set; }

        /// <summary>
        /// Custom application data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; set; }
    }
}
