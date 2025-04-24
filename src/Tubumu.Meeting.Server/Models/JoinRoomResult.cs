namespace Tubumu.Meeting.Server
{
    public class JoinRoomResult
    {
        public Peer SelfPeer { get; init; }

        public Peer[] Peers { get; init; }
    }
}
