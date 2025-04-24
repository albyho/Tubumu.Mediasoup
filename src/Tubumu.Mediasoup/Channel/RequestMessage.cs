using System;
using System.Text.Json.Serialization;
using FBS.Notification;
using FBS.Request;

namespace Tubumu.Mediasoup
{
    public class RequestMessage
    {
        #region Request

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? Id { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Method? Method { get; init; }

        #endregion

        #region Notification

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Event? Event { get; set; }

        #endregion

        #region Common

        public string? HandlerId { get; set; }

        public ArraySegment<byte> Payload { get; init; }

        #endregion
    }
}
