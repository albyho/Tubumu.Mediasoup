using System.Text.Json.Serialization;

namespace Tubumu.Mediasoup
{
    public class NotifyData
    {
        [JsonPropertyName("ppid")]
        public int PPID { get; set; }
    }
}
