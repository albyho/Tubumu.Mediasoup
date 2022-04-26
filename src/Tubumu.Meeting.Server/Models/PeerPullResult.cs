using System.Collections.Generic;
using Tubumu.Mediasoup;

namespace Tubumu.Meeting.Server
{
    public class PeerPullResult
    {
        public Producer[] ExistsProducers { get; set; }

        public HashSet<string> ProduceSources { get; set; }
    }
}
