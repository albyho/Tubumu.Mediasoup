namespace Tubumu.Mediasoup
{
    public class AudioLevelObserverVolume
    {
        /// <summary>
        /// The audio producer instance.
        /// </summary>
        public Producer Producer { get; set; }

        /// <summary>
        /// The average volume (in dBvo from -127 to 0) of the audio producer in the
        /// last interval.
        /// </summary>
        public int Volume { get; set; }
    }
}
