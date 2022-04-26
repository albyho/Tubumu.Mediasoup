using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    public enum DataConsumerType
    {
        [EnumMember(Value = "sctp")]
        Sctp,

        [EnumMember(Value = "direct")]
        Direct
    }
}
