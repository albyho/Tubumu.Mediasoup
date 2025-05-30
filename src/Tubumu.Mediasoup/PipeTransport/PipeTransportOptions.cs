﻿using System.Collections.Generic;
using FBS.SctpParameters;
using FBS.Transport;

namespace Tubumu.Mediasoup
{
    public class PipeTransportOptions
    {
        /// <summary>
        /// Listening Information.
        /// </summary>
        public ListenInfoT ListenInfo { get; init; }

        /// <summary>
        /// Create a SCTP association. Default false.
        /// </summary>
        public bool EnableSctp { get; init; }

        /// <summary>
        /// SCTP streams number.
        /// </summary>
        public NumSctpStreamsT NumSctpStreams { get; init; } = new NumSctpStreamsT { Os = 1024, Mis = 1024 };

        /// <summary>
        /// Maximum allowed size for SCTP messages sent by DataProducers.
        /// Default 268435456.
        /// </summary>
        public uint MaxSctpMessageSize { get; set; } = 268435456;

        /// <summary>
        /// Maximum SCTP send buffer used by DataConsumers.
        /// Default 268435456.
        /// </summary>
        public uint SctpSendBufferSize { get; set; } = 268435456;

        /// <summary>
        /// Enable RTX and NACK for RTP retransmission. Useful if both Routers are
        /// located in different hosts and there is packet lost in the link. For this
        /// to work, both PipeTransports must enable this setting. Default false.
        /// </summary>
        public bool EnableRtx { get; init; }

        /// <summary>
        /// Enable SRTP. Useful to protect the RTP and RTCP traffic if both Routers
        /// are located in different hosts. For this to work, connect() must be called
        /// with remote SRTP parameters. Default false.
        /// </summary>
        public bool EnableSrtp { get; init; }

        /// <summary>
        /// Custom application data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; set; }
    }
}
