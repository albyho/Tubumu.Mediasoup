namespace FBS.RtpPacket
{
    public class DumpT
    {
        public byte PayloadType { get; set; }
        public ushort SequenceNumber { get; set; }

        public uint Timestamp { get; set; }

        public bool Marker { get; set; }

        public uint Ssrc { get; set; }

        public bool IsKeyFrame { get; set; }

        public ulong Size { get; set; }

        public ulong PayloadSize { get; set; }

        public byte SpatialLayer { get; set; }

        public byte TemporalLayer { get; set; }

        public string Mid { get; set; }

        public string Rid { get; set; }

        public string Rrid { get; set; }

        public ushort? WideSequenceNumber { get; set; }
    }
}
