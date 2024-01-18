// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.Producer
{
    public struct SrTraceInfo : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static SrTraceInfo GetRootAsSrTraceInfo(ByteBuffer _bb) { return GetRootAsSrTraceInfo(_bb, new SrTraceInfo()); }
        public static SrTraceInfo GetRootAsSrTraceInfo(ByteBuffer _bb, SrTraceInfo obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public SrTraceInfo __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public uint Ssrc { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public uint NtpSec { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public uint NtpFrac { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public uint RtpTs { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public uint PacketCount { get { int o = __p.__offset(12); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public uint OctetCount { get { int o = __p.__offset(14); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }

        public static Offset<FBS.Producer.SrTraceInfo> CreateSrTraceInfo(FlatBufferBuilder builder,
            uint ssrc = 0,
            uint ntp_sec = 0,
            uint ntp_frac = 0,
            uint rtp_ts = 0,
            uint packet_count = 0,
            uint octet_count = 0)
        {
            builder.StartTable(6);
            SrTraceInfo.AddOctetCount(builder, octet_count);
            SrTraceInfo.AddPacketCount(builder, packet_count);
            SrTraceInfo.AddRtpTs(builder, rtp_ts);
            SrTraceInfo.AddNtpFrac(builder, ntp_frac);
            SrTraceInfo.AddNtpSec(builder, ntp_sec);
            SrTraceInfo.AddSsrc(builder, ssrc);
            return SrTraceInfo.EndSrTraceInfo(builder);
        }

        public static void StartSrTraceInfo(FlatBufferBuilder builder) { builder.StartTable(6); }
        public static void AddSsrc(FlatBufferBuilder builder, uint ssrc) { builder.AddUint(0, ssrc, 0); }
        public static void AddNtpSec(FlatBufferBuilder builder, uint ntpSec) { builder.AddUint(1, ntpSec, 0); }
        public static void AddNtpFrac(FlatBufferBuilder builder, uint ntpFrac) { builder.AddUint(2, ntpFrac, 0); }
        public static void AddRtpTs(FlatBufferBuilder builder, uint rtpTs) { builder.AddUint(3, rtpTs, 0); }
        public static void AddPacketCount(FlatBufferBuilder builder, uint packetCount) { builder.AddUint(4, packetCount, 0); }
        public static void AddOctetCount(FlatBufferBuilder builder, uint octetCount) { builder.AddUint(5, octetCount, 0); }
        public static Offset<FBS.Producer.SrTraceInfo> EndSrTraceInfo(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.Producer.SrTraceInfo>(o);
        }
        public SrTraceInfoT UnPack()
        {
            var _o = new SrTraceInfoT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(SrTraceInfoT _o)
        {
            _o.Ssrc = this.Ssrc;
            _o.NtpSec = this.NtpSec;
            _o.NtpFrac = this.NtpFrac;
            _o.RtpTs = this.RtpTs;
            _o.PacketCount = this.PacketCount;
            _o.OctetCount = this.OctetCount;
        }
        public static Offset<FBS.Producer.SrTraceInfo> Pack(FlatBufferBuilder builder, SrTraceInfoT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Producer.SrTraceInfo>);
            return CreateSrTraceInfo(
              builder,
              _o.Ssrc,
              _o.NtpSec,
              _o.NtpFrac,
              _o.RtpTs,
              _o.PacketCount,
              _o.OctetCount);
        }
    }

    public class SrTraceInfoT
    {
        [JsonPropertyName("ssrc")]
        public uint Ssrc { get; set; }
        [JsonPropertyName("ntp_sec")]
        public uint NtpSec { get; set; }
        [JsonPropertyName("ntp_frac")]
        public uint NtpFrac { get; set; }
        [JsonPropertyName("rtp_ts")]
        public uint RtpTs { get; set; }
        [JsonPropertyName("packet_count")]
        public uint PacketCount { get; set; }
        [JsonPropertyName("octet_count")]
        public uint OctetCount { get; set; }

        public SrTraceInfoT()
        {
            this.Ssrc = 0;
            this.NtpSec = 0;
            this.NtpFrac = 0;
            this.RtpTs = 0;
            this.PacketCount = 0;
            this.OctetCount = 0;
        }
    }
}