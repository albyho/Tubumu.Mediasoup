using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Trace event direction.
    /// </summary>
    public enum TraceEventDirection
    {
        [EnumMember(Value = "in")]
        In,

        [EnumMember(Value = "out")]
        Out
    }
}
