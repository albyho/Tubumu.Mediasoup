using System.Text.Json.Serialization;

namespace FBS.RtpParameters
{
    public class RtcpFeedbackT
    {
        public string Type { get; set; }

        /// <summary>
        /// parameter. Nullable.
        /// </summary>
        public string? Parameter { get; set; }
    }
}
