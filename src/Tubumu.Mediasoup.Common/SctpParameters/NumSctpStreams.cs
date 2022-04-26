namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Both OS and MIS are part of the SCTP INIT+ACK handshake. OS refers to the
    /// initial number of outgoing SCTP streams that the server side transport creates
    /// (to be used by DataConsumers), while MIS refers to the maximum number of
    /// incoming SCTP streams that the server side transport can receive (to be used
    /// by DataProducers). So, if the server side transport will just be used to
    /// create data producers (but no data consumers), OS can be low (~1). However,
    /// if data consumers are desired on the server side transport, OS must have a
    /// proper value and such a proper value depends on whether the remote endpoint
    /// supports  SCTP_ADD_STREAMS extension or not.
    ///
    /// libwebrtc (Chrome, Safari, etc) does not enable SCTP_ADD_STREAMS so, if data
    /// consumers are required,  OS should be 1024 (the maximum number of DataChannels
    /// that libwebrtc enables).
    ///
    /// Firefox does enable SCTP_ADD_STREAMS so, if data consumers are required, OS
    /// can be lower (16 for instance). The mediasoup transport will allocate and
    /// announce more outgoing SCTM streams when needed.
    ///
    /// mediasoup-client provides specific per browser/version OS and MIS values via
    /// the device.sctpCapabilities getter.
    /// </summary>
    public class NumSctpStreams
    {
        /// <summary>
        /// Initially requested number of outgoing SCTP streams.
        /// </summary>
        public int OS { get; set; }

        /// <summary>
        /// Maximum number of incoming SCTP streams.
        /// </summary>
        public int MIS { get; set; }
    }
}
