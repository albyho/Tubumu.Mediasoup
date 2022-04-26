namespace Tubumu.Meeting.Server
{
    public class PeerNotInRoomException : MeetingException
    {
        public PeerNotInRoomException(string tag, string peerId) : base($"{tag} | Peer:{peerId} is not in any room.")
        {

        }
    }
}
