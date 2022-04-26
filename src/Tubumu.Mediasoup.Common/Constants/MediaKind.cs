using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    public enum MediaKind
    {
        [EnumMember(Value = "audio")]
        Audio,

        [EnumMember(Value = "video")]
        Video
    }
}
