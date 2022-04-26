using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Direction of RTP header extension.
    /// </summary>
    public enum RtpHeaderExtensionDirection
    {
        [EnumMember(Value = "sendrecv")]
        SendReceive,

        [EnumMember(Value = "sendonly")]
        SendOnly,

        [EnumMember(Value = "recvonly")]
        ReceiveOnly,

        [EnumMember(Value = "inactive")]
        Inactive
    }
}
