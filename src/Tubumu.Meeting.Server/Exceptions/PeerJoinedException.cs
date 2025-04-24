namespace Tubumu.Meeting.Server
{
    public class PeerJoinedException : MeetingException
    {
        public PeerJoinedException(string tag, string peerId)
            : base($"{tag} | Peer:{peerId} was joined.") { }

        public PeerJoinedException(string message)
            : base(message) { }

        public PeerJoinedException() { }

        public PeerJoinedException(string? message, System.Exception? innerException)
            : base(message, innerException) { }
    }
}
