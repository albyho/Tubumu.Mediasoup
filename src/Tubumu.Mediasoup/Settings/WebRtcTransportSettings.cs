using System.Collections.Generic;
using FBS.Transport;

namespace Tubumu.Mediasoup
{
    public class WebRtcTransportSettings
    {
        public List<ListenInfoT> ListenInfos { get; set; }

        public uint InitialAvailableOutgoingBitrate { get; set; }

        // TODO: (alby) 貌似没有地方使用该参数。见 mediasoup\WebRtcTransport.ts 的 WebRtcTransportOptions 和 mediasoup\worker\src\Transport.cpp
        public uint MinimumAvailableOutgoingBitrate { get; set; }

        public uint MaxSctpMessageSize { get; set; }

        // Additional options that are not part of WebRtcTransportOptions.
        public uint? MaximumIncomingBitrate { get; init; }
    }
}
