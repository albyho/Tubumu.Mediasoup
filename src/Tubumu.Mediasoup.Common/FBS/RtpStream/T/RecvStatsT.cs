using System.Collections.Generic;

namespace FBS.RtpStream
{
    public class RecvStatsT
    {
        public FBS.RtpStream.StatsT Base { get; set; }

        public uint Jitter { get; set; }

        public ulong PacketCount { get; set; }

        public ulong ByteCount { get; set; }

        public uint Bitrate { get; set; }

        public List<FBS.RtpStream.BitrateByLayerT> BitrateByLayer { get; set; }
    }
}
