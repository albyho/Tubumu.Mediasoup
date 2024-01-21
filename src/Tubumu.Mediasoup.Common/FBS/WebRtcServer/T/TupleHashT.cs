using System.Text.Json.Serialization;

namespace FBS.WebRtcServer
{
    public class TupleHashT
    {
        [JsonPropertyName("tupleHash")]
        public ulong TupleHash_ { get; set; }

        public string WebRtcTransportId { get; set; }
    }
}
