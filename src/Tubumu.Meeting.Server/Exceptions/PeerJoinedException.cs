namespace Tubumu.Meeting.Server
{
    public class PeerJoinedException : MeetingException
    {
        public PeerJoinedException(string tag, string peerId) : base($"{tag} | Peer:{peerId} was joined.")
        {

        }
    }
}
