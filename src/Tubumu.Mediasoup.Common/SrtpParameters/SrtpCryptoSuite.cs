using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// SRTP crypto suite.
    /// </summary>
    public enum SrtpCryptoSuite
    {
        [EnumMember(Value = "AES_CM_128_HMAC_SHA1_80")]
        AES_CM_128_HMAC_SHA1_80,

        [EnumMember(Value = "AES_CM_128_HMAC_SHA1_32")]
        AES_CM_128_HMAC_SHA1_32
    }
}
