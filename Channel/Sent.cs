using System;

namespace TubumuMeeting.Mediasoup
{
    internal class Sent
    {
        public RequestMessage RequestMessage { get; set; }

        public Action<string> Resolve { get; set; }

        public Action<Exception> Reject { get; set; }

        public Action Close { get; set; }
    }
}
