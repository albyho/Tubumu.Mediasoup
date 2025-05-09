// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.RtpStream
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct SendStats : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static SendStats GetRootAsSendStats(ByteBuffer _bb) { return GetRootAsSendStats(_bb, new SendStats()); }
        public static SendStats GetRootAsSendStats(ByteBuffer _bb, SendStats obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public SendStats __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.RtpStream.Stats? Base { get { int o = __p.__offset(4); return o != 0 ? (FBS.RtpStream.Stats?)(new FBS.RtpStream.Stats()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public ulong PacketCount { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong ByteCount { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public uint Bitrate { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }

        public static Offset<FBS.RtpStream.SendStats> CreateSendStats(FlatBufferBuilder builder,
            Offset<FBS.RtpStream.Stats> @baseOffset = default(Offset<FBS.RtpStream.Stats>),
            ulong packet_count = 0,
            ulong byte_count = 0,
            uint bitrate = 0)
        {
            builder.StartTable(4);
            SendStats.AddByteCount(builder, byte_count);
            SendStats.AddPacketCount(builder, packet_count);
            SendStats.AddBitrate(builder, bitrate);
            SendStats.AddBase(builder, @baseOffset);
            return SendStats.EndSendStats(builder);
        }

        public static void StartSendStats(FlatBufferBuilder builder) { builder.StartTable(4); }
        public static void AddBase(FlatBufferBuilder builder, Offset<FBS.RtpStream.Stats> baseOffset) { builder.AddOffset(0, baseOffset.Value, 0); }
        public static void AddPacketCount(FlatBufferBuilder builder, ulong packetCount) { builder.AddUlong(1, packetCount, 0); }
        public static void AddByteCount(FlatBufferBuilder builder, ulong byteCount) { builder.AddUlong(2, byteCount, 0); }
        public static void AddBitrate(FlatBufferBuilder builder, uint bitrate) { builder.AddUint(3, bitrate, 0); }
        public static Offset<FBS.RtpStream.SendStats> EndSendStats(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // base
            return new Offset<FBS.RtpStream.SendStats>(o);
        }
        public SendStatsT UnPack()
        {
            var _o = new SendStatsT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(SendStatsT _o)
        {
            _o.Base = this.Base.HasValue ? this.Base.Value.UnPack() : null;
            _o.PacketCount = this.PacketCount;
            _o.ByteCount = this.ByteCount;
            _o.Bitrate = this.Bitrate;
        }
        public static Offset<FBS.RtpStream.SendStats> Pack(FlatBufferBuilder builder, SendStatsT _o)
        {
            if(_o == null)
                return default(Offset<FBS.RtpStream.SendStats>);
            var _base = _o.Base == null ? default(Offset<FBS.RtpStream.Stats>) : FBS.RtpStream.Stats.Pack(builder, _o.Base);
            return CreateSendStats(
              builder,
              _base,
              _o.PacketCount,
              _o.ByteCount,
              _o.Bitrate);
        }
    }

    public class SendStatsT
    {
        public FBS.RtpStream.StatsT Base { get; set; }

        public ulong PacketCount { get; set; }

        public ulong ByteCount { get; set; }

        public uint Bitrate { get; set; }

        public SendStatsT()
        {
            this.Base = null;
            this.PacketCount = 0;
            this.ByteCount = 0;
            this.Bitrate = 0;
        }
    }


    static public class SendStatsVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyTable(tablePos, 4 /*Base*/, FBS.RtpStream.StatsVerify.Verify, true)
              && verifier.VerifyField(tablePos, 6 /*PacketCount*/, 8 /*ulong*/, 8, false)
              && verifier.VerifyField(tablePos, 8 /*ByteCount*/, 8 /*ulong*/, 8, false)
              && verifier.VerifyField(tablePos, 10 /*Bitrate*/, 4 /*uint*/, 4, false)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
