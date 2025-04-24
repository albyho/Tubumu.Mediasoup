using System;
using FBS.Response;

namespace Tubumu.Mediasoup
{
    public class Sent
    {
        public RequestMessage RequestMessage { get; init; }

        public Action<Response> Resolve { get; init; }

        public Action<Exception> Reject { get; init; }

        public Action Close { get; init; }
    }
}
