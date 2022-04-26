namespace Tubumu.Meeting.Server
{
    public class JoinRoomResult
    {
        public Peer SelfPeer { get; set; }

        public Peer[] Peers { get; set; }
    }
}
