using System;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Provides information relating to supported header extensions. The list of
    /// RTP header extensions supported by mediasoup is defined in the
    /// supportedRtpCapabilities.ts file.
    ///
    /// mediasoup does not currently support encrypted RTP header extensions. The
    /// direction field is just present in mediasoup RTP capabilities (retrieved via
    /// router.rtpCapabilities or mediasoup.getSupportedRtpCapabilities()). It's
    /// ignored if present in endpoints' RTP capabilities.
    /// </summary>
    [Serializable]
    public class RtpHeaderExtension
    {
        /// <summary>
        /// Media kind.
        /// Default any media kind.
        /// </summary>
        public MediaKind Kind { get; set; }

        /// <summary>
        /// The URI of the RTP header extension, as defined in RFC 5285.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// The preferred numeric identifier that goes in the RTP packet. Must be
        /// unique.
        /// </summary>
        public int PreferredId { get; set; }

        /// <summary>
        /// If true, it is preferred that the value in the header be encrypted as per
        /// RFC 6904. Default false.
        /// </summary>
        public bool? PreferredEncrypt { get; set; } = false;

        /// <summary>
        /// If 'sendrecv', mediasoup supports sending and receiving this RTP extension.
        /// 'sendonly' means that mediasoup can send (but not receive) it. 'recvonly'
        /// means that mediasoup can receive (but not send) it.
        /// </summary>
        public RtpHeaderExtensionDirection? Direction { get; set; }
    }
}
