using System.Collections.Generic;
using FBS.SctpParameters;
using FBS.SrtpParameters;

namespace Tubumu.Mediasoup
{
    public class PlainTransportOptions
    {
        /// <summary>
        /// Listening information.
        /// </summary>
        public TransportListenInfo ListenInfo { get; set; }

        /// <summary>
        /// RTCP listening information. If not given and rtcpPort is not false,
        /// RTCP will use same listening info than RTP.
        /// </summary>
        public TransportListenInfo? RtcpListenInfo { get; set; }

        /// <summary>
        /// Use RTCP-mux (RTP and RTCP in the same port). Default true.
        /// </summary>
        public bool? RtcpMux { get; set; } = true;

        /// <summary>
        /// Whether remote IP:port should be auto-detected based on first RTP/RTCP
        /// packet received. If enabled, connect() method must not be called unless
        /// SRTP is enabled. If so, it must be called with just remote SRTP parameters.
        /// Default false.
        /// </summary>
        public bool? Comedia { get; set; } = false;

        /// <summary>
        /// Create a SCTP association. Default false.
        /// </summary>
        public bool? EnableSctp { get; set; } = false;

        /// <summary>
        /// SCTP streams number.
        /// </summary>
        public NumSctpStreamsT? NumSctpStreams { get; set; }

        /// <summary>
        /// Maximum allowed size for SCTP messages sent by DataProducers.
        /// Default 262144.
        /// </summary>
        public uint? MaxSctpMessageSize { get; set; } = 262144;

        /// <summary>
        /// Maximum SCTP send buffer used by DataConsumers.
        /// Default 262144.
        /// </summary>
        public uint? SctpSendBufferSize { get; set; } = 262144;

        /// <summary>
        /// Enable SRTP. For this to work, connect() must be called
        /// with remote SRTP parameters. Default false.
        /// </summary>
        public bool? EnableSrtp { get; set; } = false;

        /// <summary>
        /// The SRTP crypto suite to be used if enableSrtp is set. Default
        /// 'AES_CM_128_HMAC_SHA1_80'.
        /// </summary>
        public SrtpCryptoSuite? SrtpCryptoSuite { get; set; } = FBS.SrtpParameters.SrtpCryptoSuite.AES_CM_128_HMAC_SHA1_80;

        /// <summary>
        /// Custom application data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; set; }
    }
}
