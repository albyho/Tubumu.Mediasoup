using System.Collections.Generic;
using Tubumu.Mediasoup;

namespace Tubumu.Meeting.Server
{
    public class PullResult
    {
        public Peer ConsumePeer { get; init; }

        public Peer ProducePeer { get; init; }

        public Producer[] ExistsProducers { get; init; }

        public HashSet<string> Sources { get; init; }
    }
}
