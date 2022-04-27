using System.Text.Json.Serialization;

namespace Tubumu.Mediasoup
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
        [JsonPropertyName("ru_utime")]
        public ulong UTime { get; set; }

        /// <summary>
        /// System CPU time used (in ms).
        /// </summary>
        [JsonPropertyName("ru_stime")]
        public ulong STime { get; set; }

        /// <summary>
        /// Maximum resident set size.
        /// </summary>
        [JsonPropertyName("ru_maxrss")]
        public ulong MaxRss { get; set; }

        /// <summary>
        /// Integral shared memory size.
        /// </summary>
        [JsonPropertyName("ru_ixrss")]
        public ulong IxRss { get; set; }

        /// <summary>
        /// Integral unshared data size.
        /// </summary>
        [JsonPropertyName("ru_idrss")]
        public ulong IdRss { get; set; }

        /// <summary>
        /// Integral unshared stack size.
        /// </summary>
        [JsonPropertyName("ru_isrss")]
        public ulong IsRss { get; set; }

        /// <summary>
        /// Page reclaims (soft page faults).
        /// </summary>
        [JsonPropertyName("ru_minflt")]
        public ulong MinFlt { get; set; }

        /// <summary>
        /// Page faults (hard page faults).
        /// </summary>
        [JsonPropertyName("ru_majflt")]
        public ulong MajFlt { get; set; }

        /// <summary>
        /// Swaps.
        /// </summary>
        [JsonPropertyName("ru_nswap")]
        public ulong NSwap { get; set; }

        /// <summary>
        /// Block input operations.
        /// </summary>
        [JsonPropertyName("ru_inblock")]
        public ulong InBlock { get; set; }

        /// <summary>
        /// Block output operations.
        /// </summary>
        [JsonPropertyName("ru_oublock")]
        public ulong OuBlock { get; set; }

        /// <summary>
        /// IPC messages sent.
        /// </summary>
        [JsonPropertyName("ru_msgsnd")]
        public ulong MsgSnd { get; set; }

        /// <summary>
        /// IPC messages received.
        /// </summary>
        [JsonPropertyName("ru_msgrcv")]
        public ulong MsgRcv { get; set; }

        /// <summary>
        /// Signals received.
        /// </summary>
        [JsonPropertyName("ru_nsignals")]
        public ulong NSignals { get; set; }

        /// <summary>
        /// Voluntary context switches.
        /// </summary>
        [JsonPropertyName("ru_nvcsw")]
        public ulong NVcSw { get; set; }

        /// <summary>
        /// Involuntary context switches.
        /// </summary>
        [JsonPropertyName("ru_nivcsw")]
        public ulong NIvcSw { get; set; }
    }
}
