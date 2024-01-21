namespace FBS.Producer
{
    public class TraceNotificationT
    {
        public FBS.Producer.TraceEventType Type { get; set; }

        public ulong Timestamp { get; set; }

        public FBS.Common.TraceDirection Direction { get; set; }

        public FBS.Producer.TraceInfoUnion Info { get; set; }
    }
}
