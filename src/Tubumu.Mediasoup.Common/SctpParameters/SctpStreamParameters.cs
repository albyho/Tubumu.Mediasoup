namespace Tubumu.Mediasoup
{
    /// <summary>
    /// SCTP stream parameters describe the reliability of a certain SCTP stream.
    /// If ordered is true then maxPacketLifeTime and maxRetransmits must be
    /// false.
    /// If ordered if false, only one of maxPacketLifeTime or maxRetransmits
    /// can be true.
    /// </summary>
    public class SctpStreamParameters
    {
        /// <summary>
        /// SCTP stream id.
        /// </summary>
        public int StreamId { get; set; }

        /// <summary>
        /// Whether data messages must be received in order. If true the messages will
        /// be sent reliably. Default true.
        /// </summary>
        public bool? Ordered { get; set; } = true;

        /// <summary>
        /// When ordered is false indicates the time (in milliseconds) after which a
        /// SCTP packet will stop being retransmitted.
        /// </summary>
        public int? MaxPacketLifeTime { get; set; }

        /// <summary>
        /// When ordered is false indicates the maximum number of times a packet will
        /// be retransmitted.
        /// </summary>
        public int? MaxRetransmits { get; set; }
    }
}
