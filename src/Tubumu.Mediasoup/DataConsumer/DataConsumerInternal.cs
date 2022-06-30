namespace Tubumu.Mediasoup
{
    public class DataConsumerInternal
    {
        /// <summary>
        /// Router id.
        /// </summary>
        public string RouterId { get; }

        /// <summary>
        /// Transport id.
        /// </summary>
        public string TransportId { get; }

        /// <summary>
        /// DataConsumer id.
        /// </summary>
        public string DataConsumerId { get; }

        public DataConsumerInternal(string routerId, string transportId, string dataConsumerId)
        {
            RouterId = routerId;
            TransportId = transportId;
            DataConsumerId = dataConsumerId;
        }
    }
}

