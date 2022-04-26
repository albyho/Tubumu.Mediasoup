namespace Tubumu.Mediasoup
{
    public class ProducerVideoOrientation
    {
        /// <summary>
        /// Whether the source is a video camera.
        /// </summary>
        public bool Camera { get; set; }

        /// <summary>
        /// Whether the video source is flipped.
        /// </summary>
        public bool Flip { get; set; }

        /// <summary>
        /// Rotation degrees (0, 90, 180 or 270).
        /// </summary>
        public int Rotation { get; set; }
    }
}
