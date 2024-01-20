using System.Text.Json.Serialization;

namespace FBS.Worker
{
    public class ResourceUsageResponseT
    {
        [JsonPropertyName("ru_utime")]
        public ulong RuUtime { get; set; }

        [JsonPropertyName("ru_stime")]
        public ulong RuStime { get; set; }

        [JsonPropertyName("ru_maxrss")]
        public ulong RuMaxrss { get; set; }

        [JsonPropertyName("ru_ixrss")]
        public ulong RuIxrss { get; set; }

        [JsonPropertyName("ru_idrss")]
        public ulong RuIdrss { get; set; }

        [JsonPropertyName("ru_isrss")]
        public ulong RuIsrss { get; set; }

        [JsonPropertyName("ru_minflt")]
        public ulong RuMinflt { get; set; }

        [JsonPropertyName("ru_majflt")]
        public ulong RuMajflt { get; set; }

        [JsonPropertyName("ru_nswap")]
        public ulong RuNswap { get; set; }

        [JsonPropertyName("ru_inblock")]
        public ulong RuInblock { get; set; }

        [JsonPropertyName("ru_oublock")]
        public ulong RuOublock { get; set; }

        [JsonPropertyName("ru_msgsnd")]
        public ulong RuMsgsnd { get; set; }

        [JsonPropertyName("ru_msgrcv")]
        public ulong RuMsgrcv { get; set; }

        [JsonPropertyName("ru_nsignals")]
        public ulong RuNsignals { get; set; }

        [JsonPropertyName("ru_nvcsw")]
        public ulong RuNvcsw { get; set; }

        [JsonPropertyName("ru_nivcsw")]
        public ulong RuNivcsw { get; set; }
    }
}
