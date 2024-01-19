using FBS.Transport;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Transport protocol.
    /// </summary>
    public class TransportTuple
    {
        public string LocalIp { get; set; }

        public int LocalPort { get; set; }

        public string? RemoteIp { get; set; }

        public int? RemotePort { get; set; }

        public Protocol Protocol { get; set; }
    }
}
