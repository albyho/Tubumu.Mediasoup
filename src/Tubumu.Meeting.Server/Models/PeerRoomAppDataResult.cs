using System.Collections.Generic;

namespace Tubumu.Meeting.Server
{
    public class PeerRoomAppDataResult
    {
        public string SelfPeerId { get; set; }

        public Dictionary<string, object> AppData { get; set; }

        public string[] OtherPeerIds { get; set; }
    }
}
