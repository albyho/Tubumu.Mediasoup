namespace Tubumu.Mediasoup
{
    public class RtpObserverInternal
    {
        /// <summary>
        /// Router id.
        /// </summary>
        public string RouterId { get; }

        /// <summary>
        /// RtpObserver id.
        /// </summary>
        public string RtpObserverId { get; }

        public RtpObserverInternal(string routerId, string rtpObserverId)
        {
            RouterId = routerId;
            RtpObserverId = rtpObserverId;
        }
    }
}
