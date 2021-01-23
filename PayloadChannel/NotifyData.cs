using Newtonsoft.Json;

namespace TubumuMeeting.Mediasoup
{
    public class NotifyData
    {
        [JsonProperty("ppid")]
        public int PPID { get; set; }
    }
}
