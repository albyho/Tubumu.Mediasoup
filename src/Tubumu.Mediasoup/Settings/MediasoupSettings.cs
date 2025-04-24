namespace Tubumu.Mediasoup
{
    public class MediasoupSettings
    {
        public WorkerSettings WorkerSettings { get; init; }

        public RouterSettings RouterSettings { get; set; }

        public WebRtcServerSettings WebRtcServerSettings { get; init; }

        public WebRtcTransportSettings WebRtcTransportSettings { get; init; }

        public PlainTransportSettings PlainTransportSettings { get; init; }
    }
}
