using Tubumu.Mediasoup;

namespace Tubumu.Meeting.Server
{
    public class PeerProduceResult
    {
        /// <summary>
        /// Producer
        /// </summary>
        public Producer Producer { get; set; }

        /// <summary>
        /// PullPaddings
        /// </summary>
        public PullPadding[] PullPaddings { get; set; }
    }
}
