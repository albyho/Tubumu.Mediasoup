// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.Producer
{
    public struct KeyFrameTraceInfo : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static KeyFrameTraceInfo GetRootAsKeyFrameTraceInfo(ByteBuffer _bb) { return GetRootAsKeyFrameTraceInfo(_bb, new KeyFrameTraceInfo()); }
        public static KeyFrameTraceInfo GetRootAsKeyFrameTraceInfo(ByteBuffer _bb, KeyFrameTraceInfo obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public KeyFrameTraceInfo __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.RtpPacket.Dump? RtpPacket { get { int o = __p.__offset(4); return o != 0 ? (FBS.RtpPacket.Dump?)(new FBS.RtpPacket.Dump()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public bool IsRtx { get { int o = __p.__offset(6); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }

        public static Offset<FBS.Producer.KeyFrameTraceInfo> CreateKeyFrameTraceInfo(FlatBufferBuilder builder,
            Offset<FBS.RtpPacket.Dump> rtp_packetOffset = default(Offset<FBS.RtpPacket.Dump>),
            bool is_rtx = false)
        {
            builder.StartTable(2);
            KeyFrameTraceInfo.AddRtpPacket(builder, rtp_packetOffset);
            KeyFrameTraceInfo.AddIsRtx(builder, is_rtx);
            return KeyFrameTraceInfo.EndKeyFrameTraceInfo(builder);
        }

        public static void StartKeyFrameTraceInfo(FlatBufferBuilder builder) { builder.StartTable(2); }
        public static void AddRtpPacket(FlatBufferBuilder builder, Offset<FBS.RtpPacket.Dump> rtpPacketOffset) { builder.AddOffset(0, rtpPacketOffset.Value, 0); }
        public static void AddIsRtx(FlatBufferBuilder builder, bool isRtx) { builder.AddBool(1, isRtx, false); }
        public static Offset<FBS.Producer.KeyFrameTraceInfo> EndKeyFrameTraceInfo(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // rtp_packet
            return new Offset<FBS.Producer.KeyFrameTraceInfo>(o);
        }
        public KeyFrameTraceInfoT UnPack()
        {
            var _o = new KeyFrameTraceInfoT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(KeyFrameTraceInfoT _o)
        {
            _o.RtpPacket = this.RtpPacket.HasValue ? this.RtpPacket.Value.UnPack() : null;
            _o.IsRtx = this.IsRtx;
        }
        public static Offset<FBS.Producer.KeyFrameTraceInfo> Pack(FlatBufferBuilder builder, KeyFrameTraceInfoT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Producer.KeyFrameTraceInfo>);
            var _rtp_packet = _o.RtpPacket == null ? default(Offset<FBS.RtpPacket.Dump>) : FBS.RtpPacket.Dump.Pack(builder, _o.RtpPacket);
            return CreateKeyFrameTraceInfo(
              builder,
              _rtp_packet,
              _o.IsRtx);
        }
    }

    public class KeyFrameTraceInfoT
    {
        [JsonPropertyName("rtp_packet")]
        public FBS.RtpPacket.DumpT RtpPacket { get; set; }
        [JsonPropertyName("is_rtx")]
        public bool IsRtx { get; set; }

        public KeyFrameTraceInfoT()
        {
            this.RtpPacket = null;
            this.IsRtx = false;
        }
    }
}