using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    public class WebRtcServerOptions
    {
        /// <summary>
        /// Listen infos.
        /// </summary>
        public TransportListenInfo[] ListenInfos { get; set; }

        /// <summary>
        /// Custom application data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; set; }
    }
}
