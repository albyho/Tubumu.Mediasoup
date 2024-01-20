using System.Collections.Generic;

namespace FBS.Worker
{
    public class CreateWebRtcServerRequestT
    {
        public string WebRtcServerId { get; set; }

        public List<FBS.Transport.ListenInfoT> ListenInfos { get; set; }
    }
}
