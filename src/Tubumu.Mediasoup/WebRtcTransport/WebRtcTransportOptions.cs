using System.Collections.Generic;
using FBS.SctpParameters;
using FBS.Transport;

namespace Tubumu.Mediasoup
{
    public class WebRtcTransportOptionsBase
    {
        /// <summary>
        /// Listen in UDP. Default true.
        /// </summary>
        public bool? EnableUdp { get; set; } = true;

        /// <summary>
        /// Listen in TCP. Default true if webrtcServer is given, false otherwise.
        /// </summary>
        public bool? EnableTcp { get; set; } = true;

        /// <summary>
        /// Prefer UDP. Default false.
        /// </summary>
        public bool PreferUdp { get; set; }

        /// <summary>
        /// Prefer TCP. Default false.
        /// </summary>
        public bool PreferTcp { get; set; }

        /// <summary>
        /// ICE consent timeout (in seconds). If 0 it is disabled. Default 30.
        /// </summary>
        public byte IceConsentTimeout { get; set; } = 30;

        /// <summary>
        /// Initial available outgoing bitrate (in bps). Default 600000.
        /// </summary>
        public uint InitialAvailableOutgoingBitrate { get; set; } = 600000;

        /// <summary>
        /// Create a SCTP association. Default false.
        /// </summary>
        public bool EnableSctp { get; set; }

        /// <summary>
        /// SCTP streams number.
        /// </summary>
        public NumSctpStreamsT? NumSctpStreams { get; set; } = new NumSctpStreamsT { Os = 1024, Mis = 1024 };

        /// <summary>
        /// Maximum allowed size for SCTP messages sent by DataProducers.
        /// Default 262144.
        /// </summary>
        public uint MaxSctpMessageSize { get; set; } = 262144;

        /// <summary>
        /// Maximum SCTP send buffer used by DataConsumers.
        /// Default 262144.
        /// </summary>
        public uint SctpSendBufferSize { get; set; } = 262144;

        /// <summary>
        /// Custom application data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; set; }
    }

    public class WebRtcTransportListenServer
    {
        /// <summary>
        /// Instance of WebRtcServer. Mandatory unless listenIps is given.
        /// </summary>
        public WebRtcServer WebRtcServer { get; set; }
    }

    public class WebRtcTransportListenIndividual
    {
        /// <summary>
        /// Listening IP address or addresses in order of preference (first one is the
        /// preferred one).
        /// </summary>
        public ListenInfoT[]? ListenInfos { get; set; }

        /// <summary>
        /// Fixed port to listen on instead of selecting automatically from Worker's port
        /// range.
        /// </summary>
        public ushort? Port { get; set; } = 0; // mediasoup-work needs >= 0
    }

    public class WebRtcTransportOptions : WebRtcTransportOptionsBase
    {
        /// <summary>
        /// Instance of WebRtcServer. Mandatory unless listenIps is given.
        /// </summary>
        public WebRtcServer? WebRtcServer { get; set; }

        /// <summary>
        /// Listening IP address or addresses in order of preference (first one is the
        /// preferred one).
        /// </summary>
        public ListenInfoT[]? ListenInfos { get; set; }
    }
}
