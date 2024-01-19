using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    public class ActiveSpeakerObserverOptions
    {
        public ushort? Interval { get; set; } = 300;

        /// <summary>
        /// Custom application data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; set; }
    }
}
