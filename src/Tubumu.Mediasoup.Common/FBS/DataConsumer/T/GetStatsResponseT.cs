namespace FBS.DataConsumer
{
    public class GetStatsResponseT
    {
        public ulong Timestamp { get; set; }

        public string Label { get; set; }

        public string Protocol { get; set; }

        public ulong MessagesSent { get; set; }

        public ulong BytesSent { get; set; }

        public uint BufferedAmount { get; set; }
    }
}
