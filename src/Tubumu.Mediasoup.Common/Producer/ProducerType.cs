using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Producer type.
    /// </summary>
    public enum ProducerType
    {
        [EnumMember(Value = "simple")]
        Simple,

        [EnumMember(Value = "simulcast")]
        Simulcast,

        [EnumMember(Value = "svc")]
        Svc,

        [EnumMember(Value = "pipe")]
        Pipe
    }
}
