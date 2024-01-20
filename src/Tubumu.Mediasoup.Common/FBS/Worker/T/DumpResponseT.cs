using System.Collections.Generic;

namespace FBS.Worker
{
    public class DumpResponseT
    {
        public uint Pid { get; set; }

        public List<string> WebRtcServerIds { get; set; }

        public List<string> RouterIds { get; set; }

        public ChannelMessageHandlersT ChannelMessageHandlers { get; set; }

        public FBS.LibUring.DumpT Liburing { get; set; }
    }
}
