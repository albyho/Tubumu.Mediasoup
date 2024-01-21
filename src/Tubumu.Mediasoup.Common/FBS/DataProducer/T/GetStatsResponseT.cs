namespace FBS.DataProducer
{
    public class GetStatsResponseT
    {
        public ulong Timestamp { get; set; }

        public string Label { get; set; }

        public string Protocol { get; set; }

        public ulong MessagesReceived { get; set; }

        public ulong BytesReceived { get; set; }

        public uint BufferedAmount { get; set; }
    }
}
