using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    public enum DtlsState
    {
        [EnumMember(Value = "new")]
        New,

        [EnumMember(Value = "connecting")]
        Connecting,

        [EnumMember(Value = "connected")]
        Connected,
        
        [EnumMember(Value = "failed")]
        Failed,

        [EnumMember(Value = "closed")]
        Closed
    }
}
