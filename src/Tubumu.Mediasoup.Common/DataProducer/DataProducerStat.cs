namespace Tubumu.Mediasoup
{
    public class DataProducerStat
    {
        public string Type { get; set; }

        public long Timestamp { get; set; }

        public string Label { get; set; }

        public string Protocol { get; set; }

        public int MessagesReceived { get; set; }

        public int BytesReceived { get; set; }
    }
}
