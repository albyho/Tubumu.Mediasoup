using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Transport trace event type.
    /// </summary>
    public enum TransportTraceEventType
    {
        [EnumMember(Value = "probation")]
        Probation,

        [EnumMember(Value = "bwe")]
        BWE
    }
}
