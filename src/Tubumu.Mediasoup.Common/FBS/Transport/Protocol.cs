// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FBS.Transport
{

    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum Protocol : byte
    {
        [EnumMember(Value = "udp")]
        UDP = 1,

        [EnumMember(Value = "tcp")]
        TCP = 2,
    }


}
