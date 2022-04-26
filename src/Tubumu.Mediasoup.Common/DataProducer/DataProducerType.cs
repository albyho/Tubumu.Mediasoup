using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    public enum DataProducerType
    {
        [EnumMember(Value = "sctp")]
        Sctp,

        [EnumMember(Value = "direct")]
        Direct
    }
}
