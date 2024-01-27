namespace Tubumu.Meeting.Server
{
    public class PeerNotExistsException : MeetingException
    {
        public PeerNotExistsException(string tag, string peerId) : base($"{tag} | Peer:{peerId} is not exists.")
        {
        }

        public PeerNotExistsException(string message) : base(message)
        {
        }

        public PeerNotExistsException()
        {
        }

        public PeerNotExistsException(string? message, System.Exception? innerException) : base(message, innerException)
        {
        }
    }
}
