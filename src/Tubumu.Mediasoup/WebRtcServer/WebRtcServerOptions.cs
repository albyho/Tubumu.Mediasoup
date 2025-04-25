using System.Collections.Generic;
using FBS.Transport;

namespace Tubumu.Mediasoup
{
    public class WebRtcServerOptions
    {
        /// <summary>
        /// Listen infos.
        /// </summary>
        public List<ListenInfoT> ListenInfos { get; init; }

        /// <summary>
        /// Custom application data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; init; }
    }
}
