using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    public enum IceState
    {
        [EnumMember(Value = "new")]
        New,

        [EnumMember(Value = "connected")]
        Connected,

        [EnumMember(Value = "completed")]
        Completed,

        [EnumMember(Value = "disconnected")]
        Disconnected,

        [EnumMember(Value = "closed")]
        Closed
    }
}
