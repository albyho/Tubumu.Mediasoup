using System.Collections.Generic;

namespace FBS.WebRtcServer
{
    public class DumpResponseT
    {
        public string Id { get; set; }

        public List<IpPortT> UdpSockets { get; set; }

        public List<IpPortT> TcpServers { get; set; }

        public List<string> WebRtcTransportIds { get; set; }

        public List<IceUserNameFragmentT> LocalIceUsernameFragments { get; set; }

        public List<TupleHashT> TupleHashes { get; set; }
    }
}
