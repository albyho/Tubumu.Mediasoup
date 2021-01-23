using System;
using Newtonsoft.Json;

namespace TubumuMeeting.Mediasoup
{
    /// <summary>
    /// An object with the fields of the uv_rusage_t struct.
    /// <para>http://docs.libuv.org/en/v1.x/misc.html#c.uv_rusage_t</para>
    /// <para>https://linux.die.net/man/2/getrusage</para>
    /// </summary>
    public struct WorkerResourceUsage
    {
        /// <summary>
        /// User CPU time used (in ms).
        /// </summary>
        [JsonProperty("ru_utime")]
        public UInt64 UTime { get; set; }

        /// <summary>
        /// System CPU time used (in ms).
        /// </summary>
        [JsonProperty("ru_stime")]
        public UInt64 STime { get; set; }

        /// <summary>
        /// Maximum resident set size.
        /// </summary>
        [JsonProperty("ru_maxrss")]
        public UInt64 MaxRss { get; set; }

        /// <summary>
        /// Integral shared memory size.
        /// </summary>
        [JsonProperty("ru_ixrss")]
        public UInt64 IxRss { get; set; }

        /// <summary>
        /// Integral unshared data size.
        /// </summary>
        [JsonProperty("ru_idrss")]
        public UInt64 IdRss { get; set; }

        /// <summary>
        /// Integral unshared stack size.
        /// </summary>
        [JsonProperty("ru_isrss")]
        public UInt64 IsRss { get; set; }

        /// <summary>
        /// Page reclaims (soft page faults).
        /// </summary>
        [JsonProperty("ru_minflt")]
        public UInt64 MinFlt { get; set; }

        /// <summary>
        /// Page faults (hard page faults).
        /// </summary>
        [JsonProperty("ru_majflt")]
        public UInt64 MajFlt { get; set; }

        /// <summary>
        /// Swaps.
        /// </summary>
        [JsonProperty("ru_nswap")]
        public UInt64 NSwap { get; set; }

        /// <summary>
        /// Block input operations.
        /// </summary>
        [JsonProperty("ru_inblock")]
        public UInt64 InBlock { get; set; }

        /// <summary>
        /// Block output operations.
        /// </summary>
        [JsonProperty("ru_oublock")]
        public UInt64 OuBlock { get; set; }

        /// <summary>
        /// IPC messages sent.
        /// </summary>
        [JsonProperty("ru_msgsnd")]
        public UInt64 MsgSnd { get; set; }

        /// <summary>
        /// IPC messages received.
        /// </summary>
        [JsonProperty("ru_msgrcv")]
        public UInt64 MsgRcv { get; set; }

        /// <summary>
        /// Signals received.
        /// </summary>
        [JsonProperty("ru_nsignals")]
        public UInt64 NSignals { get; set; }

        /// <summary>
        /// Voluntary context switches.
        /// </summary>
        [JsonProperty("ru_nvcsw")]
        public UInt64 NVcSw { get; set; }

        /// <summary>
        /// Involuntary context switches.
        /// </summary>
        [JsonProperty("ru_nivcsw")]
        public UInt64 NIvcSw { get; set; }
    }
}
