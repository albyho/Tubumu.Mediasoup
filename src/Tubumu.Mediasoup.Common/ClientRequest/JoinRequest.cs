using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    public class JoinRequest
    {
        public RtpCapabilities RtpCapabilities { get; init; }

        public SctpCapabilities? SctpCapabilities { get; set; }

        public string DisplayName { get; init; }

        public string[]? Sources { get; init; }

        public Dictionary<string, object>? AppData { get; init; }
    }
}
