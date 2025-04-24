using Tubumu.Mediasoup;

namespace Tubumu.Meeting.Server
{
    public class ProduceResult
    {
        /// <summary>
        /// ProducerPeer
        /// </summary>
        public Peer ProducerPeer { get; init; }

        /// <summary>
        /// Producer
        /// </summary>
        public Producer Producer { get; init; }

        /// <summary>
        /// PullPaddingConsumerPeers
        /// </summary>
        public Peer[] PullPaddingConsumerPeers { get; init; }
    }
}
