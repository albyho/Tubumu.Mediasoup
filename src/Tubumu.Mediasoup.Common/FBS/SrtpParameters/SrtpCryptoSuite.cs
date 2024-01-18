// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FBS.SrtpParameters
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum SrtpCryptoSuite : byte
    {

        [EnumMember(Value = "AEAD_AES_256_GCM")]
        AEAD_AES_256_GCM = 0,

        [EnumMember(Value = "AEAD_AES_128_GCM")]
        AEAD_AES_128_GCM = 1,

        [EnumMember(Value = "AES_CM_128_HMAC_SHA1_80")]
        AES_CM_128_HMAC_SHA1_80 = 2,

        [EnumMember(Value = "AES_CM_128_HMAC_SHA1_32")]
        AES_CM_128_HMAC_SHA1_32 = 3,
    }
}