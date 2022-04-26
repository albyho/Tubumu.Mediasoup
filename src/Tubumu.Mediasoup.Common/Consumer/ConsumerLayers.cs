namespace Tubumu.Mediasoup
{
    public class ConsumerLayers
    {
        /// <summary>
        /// The spatial layer index (from 0 to N).
        /// </summary>
        public int SpatialLayer { get; set; }

        /// <summary>
        /// The temporal layer index (from 0 to N).
        /// </summary>
        public int? TemporalLayer { get; set; }
    }
}
