// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FBS.SctpAssociation
{

    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum SctpState : byte
    {
        [EnumMember(Value = "new")]
        NEW = 0,

        [EnumMember(Value = "connecting")]
        CONNECTING = 1,

        [EnumMember(Value = "connected")]
        CONNECTED = 2,

        [EnumMember(Value = "failed")]
        FAILED = 3,

        [EnumMember(Value = "closed")]
        CLOSED = 4,
    }



}
