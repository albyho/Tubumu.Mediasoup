﻿using System.Text.Json.Serialization;

namespace Tubumu.Meeting.Server
{
    /// <summary>
    /// MeetingNotification
    /// </summary>
    public class MeetingNotification
    {
        public string Type { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }

        public static string Stringify(string type, string? data = null)
        {
            if (data == null)
            {
                return $"{{\"type\":{type}}}";
            }
            return $"{{\"type\":{type},\"data\":{data}}}";
        }
    }
}
