namespace Tubumu.Mediasoup
{
    public class ConsumerInternal
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
        /// Consumer id.
        /// </summary>
        public string ConsumerId { get; }

        public ConsumerInternal(string routerId, string transportId, string consumerId)
        {
            RouterId = routerId;
            TransportId = transportId;
            ConsumerId = consumerId;
        }
    }
}

