namespace Tubumu.Mediasoup
{
    public class MediasoupSettings
    {
        public WorkerSettings WorkerSettings { get; set; }

        public RouterSettings RouterSettings { get; set; }

        public WebRtcTransportSettings WebRtcTransportSettings { get; set; }

        public PlainTransportSettings PlainTransportSettings { get; set; }
    }
}
