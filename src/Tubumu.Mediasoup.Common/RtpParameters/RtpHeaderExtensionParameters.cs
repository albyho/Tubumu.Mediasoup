using System;
using System.Collections.Generic;
using FBS.RtpParameters;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// <para>
    /// Defines a RTP header extension within the RTP parameters. The list of RTP
    /// header extensions supported by mediasoup is defined in the
    /// supportedRtpCapabilities.ts file.
    /// </para>
    /// <para>
    /// mediasoup does not currently support encrypted RTP header extensions and no
    /// parameters are currently considered.
    /// </para>
    /// </summary>
    [Serializable]
    public class RtpHeaderExtensionParameters
    {
        /// <summary>
        /// The URI of the RTP header extension, as defined in RFC 5285.
        /// </summary>
        public RtpHeaderExtensionUri Uri { get; set; }

        /// <summary>
        /// The numeric identifier that goes in the RTP packet. Must be unique.
        /// </summary>
        public byte Id { get; set; }

        /// <summary>
        /// If true, the value in the header is encrypted as per RFC 6904. Default false.
        /// </summary>
        public bool Encrypt { get; set; }

        /// <summary>
        /// Configuration parameters for the header extension.
        /// </summary>
        public IDictionary<string, object>? Parameters { get; set; }
    }
}
