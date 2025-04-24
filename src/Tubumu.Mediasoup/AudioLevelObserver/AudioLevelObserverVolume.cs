namespace Tubumu.Mediasoup
{
    public class AudioLevelObserverVolume
    {
        /// <summary>
        /// The audio Producer instance.
        /// </summary>
        public Producer Producer { get; init; }

        /// <summary>
        /// The average volume (in dBvo from -127 to 0) of the audio Producer in the
        /// last interval.
        /// </summary>
        public int Volume { get; init; }
    }
}
