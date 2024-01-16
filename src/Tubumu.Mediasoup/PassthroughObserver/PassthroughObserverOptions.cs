using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    public class PassthroughObserverOptions
    {
        /// <summary>
        /// Custom application data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; set; } = new Dictionary<string, object>();
    }
}
