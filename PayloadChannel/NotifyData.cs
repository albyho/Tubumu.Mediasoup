using Newtonsoft.Json;

namespace Tubumu.Mediasoup
{
    public class NotifyData
    {
        [JsonProperty("ppid")]
        public int PPID { get; set; }
    }
}
