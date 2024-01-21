using FBS.Transport;

namespace Tubumu.Mediasoup
{
    public class PlainTransportSettings
    {
        public ListenInfoT ListenInfo { get; set; }

        public uint MaxSctpMessageSize { get; set; }
    }
}
