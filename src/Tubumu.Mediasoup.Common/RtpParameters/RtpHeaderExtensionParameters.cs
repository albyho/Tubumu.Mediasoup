using System;
using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Defines a RTP header extension within the RTP parameters. The list of RTP
    /// header extensions supported by mediasoup is defined in the
    /// supportedRtpCapabilities.ts file.
    ///
    /// mediasoup does not currently support encrypted RTP header extensions and no
    /// parameters are currently considered.
    /// </summary>
    [Serializable]
    public class RtpHeaderExtensionParameters
    {
        /// <summary>
        /// The URI of the RTP header extension, as defined in RFC 5285.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// The numeric identifier that goes in the RTP packet. Must be unique.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// If true, the value in the header is encrypted as per RFC 6904. Default false.
        /// </summary>
        public bool? Encrypt { get; set; } = false;

        /// <summary>
        /// Configuration parameters for the header extension.
        /// </summary>
        public IDictionary<string, object>? Parameters { get; set; }
    }
}
