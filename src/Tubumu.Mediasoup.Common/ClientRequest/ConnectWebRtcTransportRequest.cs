using FBS.WebRtcTransport;

namespace Tubumu.Mediasoup
{
    public class ConnectWebRtcTransportRequest : ConnectRequestT
    {
        public string TransportId { get; set; }
    }
}
