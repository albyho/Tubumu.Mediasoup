using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// SCTP state.
    /// </summary>
    public enum SctpState
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
