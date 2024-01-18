using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// SCTP state.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
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
