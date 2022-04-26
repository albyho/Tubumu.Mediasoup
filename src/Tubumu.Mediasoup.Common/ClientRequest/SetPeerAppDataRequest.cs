using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    public class SetPeerAppDataRequest
    {
        public Dictionary<string, object> AppData { get; set; }
    }

    public class UnsetPeerAppDataRequest
    {
        public string[] Keys { get; set; }
    }

    public class SetPeerInternalDataRequest
    {
        public string PeerId { get; set; }

        public Dictionary<string, object> InternalData { get; set; }
    }

    public class UnsetPeerInternalDataRequest
    {
        public string PeerId { get; set; }

        public string[] Keys { get; set; }
    }
}
