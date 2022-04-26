using System.Text.Json.Serialization;

namespace Tubumu.Meeting.Server
{
    /// <summary>
    /// MeetingMessage
    /// </summary>
    public class MeetingMessage
    {
        public int Code { get; set; } = 200;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? InternalCode { get; set; }

        public string Message { get; set; } = "Success";

        public static MeetingMessage Failure(string? message = null)
        {
            return new MeetingMessage
            {
                Code = 400,
                Message = message ?? "Failure"
            };
        }

        public static MeetingMessage Success(string? message = null)
        {
            return new MeetingMessage
            {
                Code = 200,
                Message = message ?? "Success"
            };
        }

        public static string Stringify(int code, string message, string? data = null)
        {
            if (data == null)
            {
                return $"{{\"code\":{code},\"message\":\"{message}\"}}";
            }
            return $"{{\"code\":{code},\"message\":\"{message}\",\"data\":{data}}}";
        }
    }

    /// <summary>
    /// MeetingMessage
    /// </summary>
    public class MeetingMessage<T> : MeetingMessage
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T Data { get; set; }

        public static MeetingMessage<T> Success(T data, string? message = null)
        {
            return new MeetingMessage<T>
            {
                Code = 200,
                Message = message ?? "Success",
                Data = data
            };
        }

        public static new MeetingMessage<T> Failure(string? message = null)
        {
            return new MeetingMessage<T>
            {
                Code = 400,
                Message = message ?? "Success",
            };
        }
    }
}
