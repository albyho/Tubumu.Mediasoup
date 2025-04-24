using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    public class DataConsumerOptions
    {
        /// <summary>
        /// The id of the DataProducer to consume.
        /// </summary>
        public string DataProducerId { get; set; }

        /// <summary>
        /// Just if consuming over SCTP.
        /// Whether data messages must be received in order. If true the messages will
        /// be sent reliably. Defaults to the value in the DataProducer if it has type
        /// 'sctp' or to true if it has type 'direct'.
        /// </summary>
        public bool? Ordered { get; set; }

        /// <summary>
        /// Just if consuming over SCTP.
        /// When ordered is false indicates the time (in milliseconds) after which a
        /// SCTP packet will stop being retransmitted. Defaults to the value in the
        /// DataProducer if it has type 'sctp' or unset if it has type 'direct'.
        /// </summary>
        public ushort? MaxPacketLifeTime { get; set; }

        /// <summary>
        /// Just if consuming over SCTP.
        /// When ordered is false indicates the maximum number of times a packet will
        /// be retransmitted. Defaults to the value in the DataProducer if it has type
        /// 'sctp' or unset if it has type 'direct'.
        /// </summary>
        public ushort? MaxRetransmits { get; set; }

        /// <summary>
        /// Whether the data consumer must start in paused mode. Default false.
        /// </summary>
        /// <value></value>
        public bool Paused { get; set; }

        /**
         * Subchannels this data consumer initially subscribes to.
         * Only used in case this data consumer receives messages from a local data
         * producer that specifies subchannel(s) when calling send().
         */
        public List<ushort>? Subchannels { get; set; }

        /// <summary>
        /// Custom application data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; set; }
    }
}
