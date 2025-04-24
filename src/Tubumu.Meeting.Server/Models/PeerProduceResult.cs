using Tubumu.Mediasoup;

namespace Tubumu.Meeting.Server
{
    public class PeerProduceResult
    {
        /// <summary>
        /// Producer
        /// </summary>
        public Producer Producer { get; init; }

        /// <summary>
        /// PullPaddings
        /// </summary>
        public PullPadding[] PullPaddings { get; init; }
    }
}
