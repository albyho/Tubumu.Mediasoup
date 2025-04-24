namespace Tubumu.Meeting.Server
{
    public class PeerNotInRoomException : MeetingException
    {
        public PeerNotInRoomException(string tag, string peerId)
            : base($"{tag} | Peer:{peerId} is not in any room.") { }

        public PeerNotInRoomException(string message)
            : base(message) { }

        public PeerNotInRoomException() { }

        public PeerNotInRoomException(string? message, System.Exception? innerException)
            : base(message, innerException) { }
    }
}
