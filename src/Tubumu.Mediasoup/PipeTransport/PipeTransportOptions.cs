using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    public class PipeTransportOptions
    {
        /// <summary>
        /// Listening IP address.
        /// </summary>
        public TransportListenIp ListenIp { get; set; }

        /// <summary>
        /// Fixed port to listen on instead of selecting automatically from Worker's port
        /// range.
        /// </summary>
        public ushort? Port { get; set; } = 0; // mediasoup-work needs >= 0

        /// <summary>
        /// Create a SCTP association. Default false.
        /// </summary>
        public bool? EnableSctp { get; set; } = false;

        /// <summary>
        /// SCTP streams number.
        /// </summary>
        public NumSctpStreams? NumSctpStreams { get; set; }

        /// <summary>
        /// Maximum allowed size for SCTP messages sent by DataProducers.
        /// Default 268435456.
        /// </summary>
        public int? MaxSctpMessageSize { get; set; } = 268435456;

        /// <summary>
        /// Maximum SCTP send buffer used by DataConsumers.
        /// Default 268435456.
        /// </summary>
        public int? SctpSendBufferSize { get; set; } = 268435456;

        /// <summary>
        /// Enable RTX and NACK for RTP retransmission. Useful if both Routers are
        /// located in different hosts and there is packet lost in the link. For this
        /// to work, both PipeTransports must enable this setting. Default false.
        /// </summary>
        public bool? EnableRtx { get; set; } = false;

        /// <summary>
        /// Enable SRTP. Useful to protect the RTP and RTCP traffic if both Routers
        /// are located in different hosts. For this to work, connect() must be called
        /// with remote SRTP parameters. Default false.
        /// </summary>
        public bool? EnableSrtp { get; set; } = false;

        /// <summary>
        /// Custom application data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; set; }
    }
}
