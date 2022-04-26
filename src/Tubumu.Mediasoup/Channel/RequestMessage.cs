using System.Text.Json.Serialization;

namespace Tubumu.Mediasoup
{
    public class RequestMessage
    {
        public uint Id { get; set; }

        public string Method { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Internal { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }
    }

    // Note: For testing.
    /*
    public class ResponseMessage
    {
        public uint? Id { get; set; }

        public string? TargetId { get; set; }

        public string? Event { get; set; }

        public bool? Accepted { get; set; }

        public string? Error { get; set; }

        public string? Reason { get; set; }

        public object? Data { get; set; }
    }
    */
}
