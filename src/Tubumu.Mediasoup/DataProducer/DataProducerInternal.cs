namespace Tubumu.Mediasoup
{
    public class DataProducerInternal : TransportInternal
    {
        /// <summary>
        /// DataProducer id.
        /// </summary>
        public string DataProducerId { get; }

        public DataProducerInternal(string routerId, string transportId, string dataProducerId) : base(routerId, transportId)
        {
            DataProducerId = dataProducerId;
        }
    }
}
