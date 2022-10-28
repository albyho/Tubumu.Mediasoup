using System.Text.Json.Serialization;

namespace Tubumu.Mediasoup
{
    public class RequestMessage
    {
        #region Request

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? Id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Method { get; set; }

        #endregion

        #region Notification

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Event { get; set; }

        #endregion

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? HandlerId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Data { get; set; }

        [JsonIgnore]
        public byte[]? Payload { get; set; }
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
