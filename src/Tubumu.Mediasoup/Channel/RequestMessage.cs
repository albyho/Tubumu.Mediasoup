using System.Text.Json.Serialization;
using FBS.Notification;
using FBS.Request;

namespace Tubumu.Mediasoup
{
    public class RequestMessage
    {
        #region Request

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? Id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Method? Method { get; set; }

        #endregion

        #region Notification

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Event? Event { get; set; }

        #endregion

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? HandlerId { get; set; }

        public byte[] Payload { get; set; }
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
