using System.Collections.Generic;
using Tubumu.Mediasoup;

namespace Tubumu.Meeting.Server
{
    public class PeerPullResult
    {
        public Producer[] ExistsProducers { get; init; }

        public HashSet<string> ProduceSources { get; init; }
    }
}
