// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FBS.WebRtcTransport
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum DtlsRole : byte
    {
        [EnumMember(Value = "auto")]
        AUTO = 0,

        [EnumMember(Value = "client")]
        CLIENT = 1,

        [EnumMember(Value = "server")]
        SERVER = 2,
    }
}
