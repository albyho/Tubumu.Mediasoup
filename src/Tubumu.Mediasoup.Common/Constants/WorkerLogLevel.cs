using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Tubumu.Mediasoup
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum WorkerLogLevel
    {
        /// <summary>
        /// Log all severities.
        /// </summary>
        [EnumMember(Value = "debug")]
        Debug,

        /// <summary>
        /// Log “warn” and “error” severities.
        /// </summary>
        [EnumMember(Value = "warn")]
        Warn,

        /// <summary>
        /// Log “error” severity.
        /// </summary>
        [EnumMember(Value = "error")]
        Error,

        /// <summary>
        /// Do not log anything.
        /// </summary>
        [EnumMember(Value = "none")]
        None
    }
}
