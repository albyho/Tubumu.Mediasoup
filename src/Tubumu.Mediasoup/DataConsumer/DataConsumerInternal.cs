namespace Tubumu.Mediasoup
{
    public class DataConsumerInternal : TransportInternal
    {
        /// <summary>
        /// DataConsumer id.
        /// </summary>
        public string DataConsumerId { get; }

        public DataConsumerInternal(string routerId, string transportId, string dataConsumerId) : base(routerId, transportId)
        {
            DataConsumerId = dataConsumerId;
        }
    }
}
