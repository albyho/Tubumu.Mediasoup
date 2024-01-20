using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    public class AudioLevelObserverOptions
    {
        /// <summary>
        /// Maximum number of entries in the 'volumes”' event. Default 1.
        /// </summary>
        public ushort MaxEntries { get; set; } = 1;

        /// <summary>
        /// Minimum average volume (in dBvo from -127 to 0) for entries in the
        /// 'volumes' event. Default -80.
        /// </summary>
        public sbyte Threshold { get; set; } = -80;

        /// <summary>
        /// Interval in ms for checking audio volumes. Default 1000.
        /// </summary>
        public ushort Interval { get; set; } = 1000;

        /// <summary>
        /// Custom application data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; set; } = new Dictionary<string, object>();
    }
}
