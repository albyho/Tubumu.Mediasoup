namespace Tubumu.Mediasoup
{
    public class DataConsumerStat
    {
        public string Type { get; set; }

        public long Timestamp { get; set; }

        public string Label { get; set; }

        public string Protocol { get; set; }

        public int MessagesSent { get; set; }

        public int BytesSent { get; set; }
    }
}
