namespace Tubumu.Meeting.Server
{
    public class LeaveResult
    {
        public Peer SelfPeer { get; init; }

        public string[] OtherPeerIds { get; set; }
    }
}
