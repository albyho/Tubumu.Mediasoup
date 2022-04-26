using System.Collections.Generic;
using System.Text;

namespace Tubumu.Mediasoup
{
    public class RecorderPrepareResult
    {
        public string TransportId { get; set; }

        public string Ip { get; set; }

        public int Port { get; set; }

        public int? RtcpPort { get; set; }

        public List<ConsumerParameters> ConsumerParameters { get; set; } = new List<ConsumerParameters>();

        public string Sdp(int index)
        {
            if (string.IsNullOrWhiteSpace(Ip) || Port == 0 || index >= ConsumerParameters.Count)
            {
                return "";
            }

            var sb = new StringBuilder();
            sb.AppendLine("v=0");
            sb.AppendLine("o=- 0 0 IN IP4 127.0.0.1");
            sb.AppendLine("s=-");
            sb.AppendLine($"c=IN IP4 {Ip}");
            sb.AppendLine("t=0 0");
            sb.AppendLine($"m=audio {Port} RTP/AVP {ConsumerParameters[0].PayloadType}");
            sb.AppendLine($"a=rtpmap:{ConsumerParameters[0].PayloadType} opus/48000/2");
            sb.AppendLine($"a=fmtp:{ConsumerParameters[0].PayloadType} minptime=10;useinbandfec=1;stereo=1");
            //sb.AppendLine("a=recvonly");
            
            if (RtcpPort.HasValue)
            {
                sb.AppendLine($"a=rtcp:{RtcpPort} IN IP4 {Ip}");
            }

            return sb.ToString();
        }
    }

    public class ConsumerParameters
    {
        public string Source { get; set; }

        public MediaKind Kind { get; set; }

        public int PayloadType { get; set; }

        public uint Ssrc { get; set; }
    }
}