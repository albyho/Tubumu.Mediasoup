namespace Tubumu.Mediasoup
{
    public class ProducerInternal : TransportInternal
    {
        /// <summary>
        /// Producer id.
        /// </summary>
        public string ProducerId { get; }

        public ProducerInternal(string routerId, string transportId, string producerId) : base(routerId, transportId)
        {
            ProducerId = producerId;
        }
    }
}

