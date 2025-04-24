namespace Tubumu.Meeting.Server
{
    public class PeerInRoomException : MeetingException
    {
        public PeerInRoomException(string tag, string peerId, string roomId)
            : base($"{tag} | Peer:{peerId} was in Room:{roomId} already.") { }

        public PeerInRoomException(string message)
            : base(message) { }

        public PeerInRoomException() { }

        public PeerInRoomException(string? message, System.Exception? innerException)
            : base(message, innerException) { }
    }
}
