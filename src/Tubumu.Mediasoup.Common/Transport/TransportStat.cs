namespace Tubumu.Mediasoup
{
    public class TransportStat
    {
        // Common to all Transports.
        public string Type { get; set; }

        public string TransportId { get; set; }

        public long Timestamp { get; set; }

        public SctpState? SctpState { get; set; }

        public int BytesReceived { get; set; }

        public int RecvBitrate { get; set; }

        public int BytesSent { get; set; }

        public int SendBitrate { get; set; }

        public int RtpBytesReceived { get; set; }

        public int RtpRecvBitrate { get; set; }

        public int RtpBytesSent { get; set; }

        public int RtpSendBitrate { get; set; }

        public int RtxBytesReceived { get; set; }

        public int RtxRecvBitrate { get; set; }

        public int RtxBytesSent { get; set; }

        public int RtxSendBitrate { get; set; }

        public int ProbationBytesSent { get; set; }

        public int ProbationSendBitrate { get; set; }

        public int? AvailableOutgoingBitrate { get; set; }

        public int? AvailableIncomingBitrate { get; set; }

        public int? MaxIncomingBitrate { get; set; }
    }
}
