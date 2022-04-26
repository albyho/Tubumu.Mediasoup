namespace Tubumu.Meeting.Server
{
    public class PeerNotExistsException : MeetingException
    {
        public PeerNotExistsException(string tag, string peerId) : base($"{tag} | Peer:{peerId} is not exists.")
        {

        }
    }
}
