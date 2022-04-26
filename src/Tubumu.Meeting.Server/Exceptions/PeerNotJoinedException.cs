namespace Tubumu.Meeting.Server
{
    public class PeerNotJoinedException : MeetingException
    {
        public PeerNotJoinedException(string tag, string peerId) : base($"{tag} | Peer:{peerId} is not joined.")
        {

        }
    }
}
