using FBS.Transport;

namespace Tubumu.Mediasoup
{
    public class WebRtcTransportSettings
    {
        public ListenInfoT[] ListenInfos { get; set; }

        public int InitialAvailableOutgoingBitrate { get; set; }

        // TODO: (alby) 貌似没有地方使用该参数。见 mediasoup\WebRtcTransport.ts 的 WebRtcTransportOptions 和 mediasoup\worker\src\Transport.cpp
        public int MinimumAvailableOutgoingBitrate { get; set; }

        public int MaxSctpMessageSize { get; set; }

        // Additional options that are not part of WebRtcTransportOptions.
        public int? MaximumIncomingBitrate { get; set; }
    }
}
