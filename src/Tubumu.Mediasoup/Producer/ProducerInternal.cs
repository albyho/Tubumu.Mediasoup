namespace Tubumu.Mediasoup
{
    public class ProducerInternal
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
        /// Producer id.
        /// </summary>
        public string ProducerId { get; }

        public ProducerInternal(string routerId, string transportId, string producerId)
        {
            RouterId = routerId;
            TransportId = transportId;
            ProducerId = producerId;
        }
    }
}

