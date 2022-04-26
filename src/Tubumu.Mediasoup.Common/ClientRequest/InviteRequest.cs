using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    public class InviteRequest
    {
        public string PeerId { get; set; }

        public HashSet<string> Sources { get; set; }
    }

    public class DeinviteRequest
    {
        public string PeerId { get; set; }

        public HashSet<string> Sources { get; set; }
    }
}
