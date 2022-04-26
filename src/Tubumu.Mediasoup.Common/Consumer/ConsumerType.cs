using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Consumer type.
    /// </summary>
    public enum ConsumerType
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
