namespace Tubumu.Mediasoup
{
    public class DataProducerInternal
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
        /// DataProducer id.
        /// </summary>
        public string DataProducerId { get; }

        public DataProducerInternal(string routerId, string transportId, string dataProducerId)
        {
            RouterId = routerId;
            TransportId = transportId;
            DataProducerId = dataProducerId;
        }
    }
}

