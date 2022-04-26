using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    public class JoinRoomRequest
    {
        public string RoomId { get; set; }

        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        [EnumMember(Value = "normal")]
        Normal,

        [EnumMember(Value = "admin")]
        Admin
    }
}
