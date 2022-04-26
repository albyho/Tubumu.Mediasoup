using System.Collections.Generic;

namespace Tubumu.Meeting.Server
{
    public class PeerAppDataResult
    {
        public string SelfPeerId { get; set; }

        public Dictionary<string, object> AppData { get; set; }

        public string[] OtherPeerIds { get; set; }
    }

    public class PeerInternalDataResult
    {
        public Dictionary<string, object> InternalData { get; set; }
    }
}
