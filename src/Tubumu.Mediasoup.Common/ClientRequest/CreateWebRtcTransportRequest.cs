namespace Tubumu.Mediasoup
{
    public class CreateWebRtcTransportRequest
    {
        public bool ForceTcp { get; set; }

        public SctpCapabilities? SctpCapabilities { get; set; }
    }
}
