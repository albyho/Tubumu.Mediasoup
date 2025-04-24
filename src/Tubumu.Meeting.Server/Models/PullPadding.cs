namespace Tubumu.Meeting.Server
{
    public class PullPadding
    {
        /// <summary>
        /// RoomId
        /// <para>因为消费 Peer 不一定全是在本次生产的对应的 Room 里发起的，故需要带上 RoomId 。</para>
        /// </summary>
        public string RoomId { get; init; }

        /// <summary>
        /// ConsumerPeerId
        /// </summary>
        public string ConsumerPeerId { get; init; }

        /// <summary>
        /// Source
        /// </summary>
        public string Source { get; init; }
    }
}
