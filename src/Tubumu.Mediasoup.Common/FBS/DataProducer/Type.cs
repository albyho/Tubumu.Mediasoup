// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FBS.DataProducer
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum Type : byte
    {
        [EnumMember(Value = "sctp")]
        SCTP = 0,

        [EnumMember(Value = "direct")]
        DIRECT = 1,
    };

}