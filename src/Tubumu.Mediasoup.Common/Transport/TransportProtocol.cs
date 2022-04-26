using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Transport protocol.
    /// </summary>
    public enum TransportProtocol
    {
        [EnumMember(Value = "udp")]
        UDP,

        [EnumMember(Value = "tcp")]
        TCP
    }
}
