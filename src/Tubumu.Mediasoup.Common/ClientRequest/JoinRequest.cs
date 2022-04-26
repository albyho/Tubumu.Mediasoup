using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    public class JoinRequest
    {
        public RtpCapabilities RtpCapabilities { get; set; }

        public SctpCapabilities? SctpCapabilities { get; set; }

        public string DisplayName { get; set; }

        public string[]? Sources { get; set; }

        public Dictionary<string, object>? AppData { get; set; }
    }
}
