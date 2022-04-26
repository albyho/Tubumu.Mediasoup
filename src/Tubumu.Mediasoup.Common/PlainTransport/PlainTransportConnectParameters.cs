using System.Text.Json.Serialization;

namespace Tubumu.Mediasoup
{
    public class PlainTransportConnectParameters
    {
        public string Ip { get; set; }

        public int Port { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? RtcpPort { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SrtpParameters? SrtpParameters { get; set; }
    }
}
