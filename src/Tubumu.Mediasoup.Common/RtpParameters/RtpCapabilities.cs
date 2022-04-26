using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// The RTP capabilities define what mediasoup or an endpoint can receive at
    /// media level.
    /// </summary>
    [Serializable]
    public class RtpCapabilities
    {
        /// <summary>
        /// Supported media and RTX codecs.
        /// </summary>
        public List<RtpCodecCapability>? Codecs { get; set; }

        /// <summary>
        /// Supported RTP header extensions.
        /// </summary>
        public RtpHeaderExtension[]? HeaderExtensions { get; set; }

        public static RtpCapabilities SupportedRtpCapabilities { get; }


        static RtpCapabilities()
        {
            SupportedRtpCapabilities = new RtpCapabilities
            {
                Codecs = new List<RtpCodecCapability>
                {
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType = "audio/opus",
                        ClockRate = 48000,
                        Channels = 2,
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback{
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType = "audio/multiopus",
                        ClockRate = 48000,
                        Channels = 4,
                        // Quad channel
                        Parameters = new Dictionary<string, object> {
                            { "channel_mapping", "0,1,2,3" },
                            { "num_streams", 2 },
                            { "coupled_streams", 2 },
                        },
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback{
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType = "audio/multiopus",
                        ClockRate = 48000,
                        Channels = 6,
                        // 5.1
                        Parameters = new Dictionary<string, object> {
                            { "channel_mapping", "0,4,1,2,3,5" },
                            { "num_streams", 4 },
                            { "coupled_streams", 2 },
                        },
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback{
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType = "audio/multiopus",
                        ClockRate = 48000,
                        Channels = 8,
                        // 7.1
                        Parameters = new Dictionary<string, object> {
                            { "channel_mapping", "0,6,1,2,3,4,5,7" },
                            { "num_streams", 5 },
                            { "coupled_streams", 4 },
                        },
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback{
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/PCMU",
                        PreferredPayloadType= 0,
                        ClockRate = 8000,
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback{
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/PCMA",
                        PreferredPayloadType= 8,
                        ClockRate = 8000,
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback{
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/ISAC",
                        ClockRate = 32000,
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback{
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/ISAC",
                        ClockRate = 16000,
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback{
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/G722",
                        PreferredPayloadType= 9,
                        ClockRate = 8000,
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback{
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/iLBC",
                        ClockRate = 8000,
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback{
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/SILK",
                        ClockRate = 24000,
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback{
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/SILK",
                        ClockRate = 16000,
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback{
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/SILK",
                        ClockRate = 12000,
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback{
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/SILK",
                        ClockRate = 8000,
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback{
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/CN",
                        PreferredPayloadType= 13,
                        ClockRate = 32000
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/CN",
                        PreferredPayloadType= 13,
                        ClockRate = 16000
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/CN",
                        PreferredPayloadType= 13,
                        ClockRate = 8000
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/telephone-event",
                        ClockRate = 48000
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/telephone-event",
                        ClockRate = 32000
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/telephone-event",
                        ClockRate = 16000
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Audio,
                        MimeType ="audio/telephone-event",
                        ClockRate = 8000
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Video,
                        MimeType ="video/VP8",
                        ClockRate = 90000,
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback {
                                Type = "nack",
                            },
                            new RtcpFeedback {
                                Type = "nack", Parameter = "pli",
                            },
                            new RtcpFeedback {
                                Type = "ccm", Parameter = "fir",
                            },
                            new RtcpFeedback {
                                Type = "goog-remb",
                            },
                            new RtcpFeedback {
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Video,
                        MimeType ="video/VP9",
                        ClockRate = 90000,
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback {
                                Type = "nack",
                            },
                            new RtcpFeedback {
                                Type = "nack", Parameter = "pli",
                            },
                            new RtcpFeedback {
                                Type = "ccm", Parameter = "fir",
                            },
                            new RtcpFeedback {
                                Type = "goog-remb",
                            },
                            new RtcpFeedback {
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Video,
                        MimeType ="video/H264",
                        ClockRate = 90000,
                        Parameters = new Dictionary<string, object> {
                            { "level-asymmetry-allowed", 1 },
                        },
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback {
                                Type = "nack",
                            },
                            new RtcpFeedback {
                                Type = "nack", Parameter = "pli",
                            },
                            new RtcpFeedback {
                                Type = "ccm", Parameter = "fir",
                            },
                            new RtcpFeedback {
                                Type = "goog-remb",
                            },
                            new RtcpFeedback {
                                Type = "transport-cc",
                            },
                        }
                    },
                    new RtpCodecCapability {
                        Kind = MediaKind.Video,
                        MimeType ="video/H265",
                        ClockRate = 90000,
                        Parameters = new Dictionary<string, object> {
                            { "level-asymmetry-allowed", 1 },
                        },
                        RtcpFeedback = new RtcpFeedback[]
                        {
                            new RtcpFeedback {
                                Type = "nack",
                            },
                            new RtcpFeedback {
                                Type = "nack", Parameter = "pli",
                            },
                            new RtcpFeedback {
                                Type = "ccm", Parameter = "fir",
                            },
                            new RtcpFeedback {
                                Type = "goog-remb",
                            },
                            new RtcpFeedback {
                                Type = "transport-cc",
                            },
                        }
                    }
                },
                HeaderExtensions = new RtpHeaderExtension[]
                {
                     new RtpHeaderExtension {
                        Kind = MediaKind.Audio,
                        Uri= "urn:ietf:params:rtp-hdrext:sdes:mid",
                        PreferredId= 1,
                        PreferredEncrypt = false,
                        Direction = RtpHeaderExtensionDirection.SendReceive
                    },
                    new RtpHeaderExtension {
                        Kind = MediaKind.Video,
                        Uri= "urn:ietf:params:rtp-hdrext:sdes:mid",
                        PreferredId= 1,
                        PreferredEncrypt = false,
                        Direction = RtpHeaderExtensionDirection.SendReceive
                    },
                    new RtpHeaderExtension {
                        Kind = MediaKind.Video,
                        Uri= "urn:ietf:params:rtp-hdrext:sdes:rtp-stream-id",
                        PreferredId= 2,
                        PreferredEncrypt = false,
                        Direction = RtpHeaderExtensionDirection.ReceiveOnly
                    },
                    new RtpHeaderExtension {
                        Kind = MediaKind.Video,
                        Uri= "urn:ietf:params:rtp-hdrext:sdes:repaired-rtp-stream-id",
                        PreferredId= 3,
                        PreferredEncrypt = false,
                        Direction = RtpHeaderExtensionDirection.ReceiveOnly
                    },
                    new RtpHeaderExtension {
                        Kind = MediaKind.Audio,
                        Uri= "http://www.webrtc.org/experiments/rtp-hdrext/abs-send-time",
                        PreferredId= 4,
                        PreferredEncrypt = false,
                        Direction = RtpHeaderExtensionDirection.SendReceive
                    },
                    new RtpHeaderExtension {
                        Kind = MediaKind.Video,
                        Uri= "http://www.webrtc.org/experiments/rtp-hdrext/abs-send-time",
                        PreferredId= 4,
                        PreferredEncrypt = false,
                        Direction = RtpHeaderExtensionDirection.SendReceive
                    },
		            // NOTE: For audio we just enable transport-wide-cc-01 when receiving media.
		            new RtpHeaderExtension {
                        Kind = MediaKind.Audio,
                        Uri= "http://www.ietf.org/id/draft-holmer-rmcat-transport-wide-cc-extensions-01",
                        PreferredId= 5,
                        PreferredEncrypt = false,
                        Direction = RtpHeaderExtensionDirection.ReceiveOnly,
                    },
                    new RtpHeaderExtension {
                        Kind = MediaKind.Video,
                        Uri= "http://www.ietf.org/id/draft-holmer-rmcat-transport-wide-cc-extensions-01",
                        PreferredId= 5,
                        PreferredEncrypt = false,
                        Direction = RtpHeaderExtensionDirection.SendReceive
                    },
		            // NOTE: Remove this once framemarking draft becomes RFC.
		            new RtpHeaderExtension {
                        Kind = MediaKind.Video,
                        Uri= "http://tools.ietf.org/html/draft-ietf-avtext-framemarking-07",
                        PreferredId= 6,
                        PreferredEncrypt = false,
                        Direction = RtpHeaderExtensionDirection.SendReceive
                    },
                    new RtpHeaderExtension {
                        Kind = MediaKind.Video,
                        Uri= "urn:ietf:params:rtp-hdrext:framemarking",
                        PreferredId= 7,
                        PreferredEncrypt = false,
                        Direction = RtpHeaderExtensionDirection.SendReceive
                    },
                    new RtpHeaderExtension {
                        Kind = MediaKind.Audio,
                        Uri= "urn:ietf:params:rtp-hdrext:ssrc-audio-level",
                        PreferredId= 10,
                        PreferredEncrypt = false,
                        Direction = RtpHeaderExtensionDirection.SendReceive
                    },
                    new RtpHeaderExtension {
                        Kind = MediaKind.Video,
                        Uri= "urn:3gpp:video-orientation",
                        PreferredId= 11,
                        PreferredEncrypt = false,
                        Direction = RtpHeaderExtensionDirection.SendReceive
                    },
                    new RtpHeaderExtension {
                        Kind = MediaKind.Video,
                        Uri= "urn:ietf:params:rtp-hdrext:toffset",
                        PreferredId= 12,
                        PreferredEncrypt = false,
                        Direction = RtpHeaderExtensionDirection.SendReceive
                    },
                    new RtpHeaderExtension {
                        Kind = MediaKind.Video,
                        Uri= "http://www.webrtc.org/experiments/rtp-hdrext/abs-capture-time",
                        PreferredId= 13,
                        PreferredEncrypt = false,
                        Direction = RtpHeaderExtensionDirection.SendReceive
                    },
                    new RtpHeaderExtension {
                        Kind = MediaKind.Audio,
                        Uri= "http://www.webrtc.org/experiments/rtp-hdrext/abs-capture-time",
                        PreferredId= 13,
                        PreferredEncrypt = false,
                        Direction = RtpHeaderExtensionDirection.SendReceive
                    }
                }
            };
        }
    }
}
