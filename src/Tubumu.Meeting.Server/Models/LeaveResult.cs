namespace Tubumu.Meeting.Server
{
    public class LeaveResult
    {
        public Peer SelfPeer { get; set; }

        public string[] OtherPeerIds { get; set; }
    }
}
