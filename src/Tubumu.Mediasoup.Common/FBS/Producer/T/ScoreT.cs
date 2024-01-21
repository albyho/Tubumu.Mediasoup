using System.Text.Json.Serialization;

namespace FBS.Producer
{
    public class ScoreT
    {
        public uint EncodingIdx { get; set; }

        public uint Ssrc { get; set; }

        public string Rid { get; set; }

        [JsonPropertyName("score")]
        public byte Score_ { get; set; }
    }
}
