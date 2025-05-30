// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.DataProducer
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct GetStatsResponse : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static GetStatsResponse GetRootAsGetStatsResponse(ByteBuffer _bb) { return GetRootAsGetStatsResponse(_bb, new GetStatsResponse()); }
        public static GetStatsResponse GetRootAsGetStatsResponse(ByteBuffer _bb, GetStatsResponse obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public GetStatsResponse __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public ulong Timestamp { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public string Label { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetLabelBytes() { return __p.__vector_as_span<byte>(6, 1); }
#else
        public ArraySegment<byte>? GetLabelBytes() { return __p.__vector_as_arraysegment(6); }
#endif
        public byte[] GetLabelArray() { return __p.__vector_as_array<byte>(6); }
        public string Protocol { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetProtocolBytes() { return __p.__vector_as_span<byte>(8, 1); }
#else
        public ArraySegment<byte>? GetProtocolBytes() { return __p.__vector_as_arraysegment(8); }
#endif
        public byte[] GetProtocolArray() { return __p.__vector_as_array<byte>(8); }
        public ulong MessagesReceived { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong BytesReceived { get { int o = __p.__offset(12); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public uint BufferedAmount { get { int o = __p.__offset(14); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }

        public static Offset<FBS.DataProducer.GetStatsResponse> CreateGetStatsResponse(FlatBufferBuilder builder,
            ulong timestamp = 0,
            StringOffset labelOffset = default(StringOffset),
            StringOffset protocolOffset = default(StringOffset),
            ulong messages_received = 0,
            ulong bytes_received = 0,
            uint buffered_amount = 0)
        {
            builder.StartTable(6);
            GetStatsResponse.AddBytesReceived(builder, bytes_received);
            GetStatsResponse.AddMessagesReceived(builder, messages_received);
            GetStatsResponse.AddTimestamp(builder, timestamp);
            GetStatsResponse.AddBufferedAmount(builder, buffered_amount);
            GetStatsResponse.AddProtocol(builder, protocolOffset);
            GetStatsResponse.AddLabel(builder, labelOffset);
            return GetStatsResponse.EndGetStatsResponse(builder);
        }

        public static void StartGetStatsResponse(FlatBufferBuilder builder) { builder.StartTable(6); }
        public static void AddTimestamp(FlatBufferBuilder builder, ulong timestamp) { builder.AddUlong(0, timestamp, 0); }
        public static void AddLabel(FlatBufferBuilder builder, StringOffset labelOffset) { builder.AddOffset(1, labelOffset.Value, 0); }
        public static void AddProtocol(FlatBufferBuilder builder, StringOffset protocolOffset) { builder.AddOffset(2, protocolOffset.Value, 0); }
        public static void AddMessagesReceived(FlatBufferBuilder builder, ulong messagesReceived) { builder.AddUlong(3, messagesReceived, 0); }
        public static void AddBytesReceived(FlatBufferBuilder builder, ulong bytesReceived) { builder.AddUlong(4, bytesReceived, 0); }
        public static void AddBufferedAmount(FlatBufferBuilder builder, uint bufferedAmount) { builder.AddUint(5, bufferedAmount, 0); }
        public static Offset<FBS.DataProducer.GetStatsResponse> EndGetStatsResponse(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 6);  // label
            builder.Required(o, 8);  // protocol
            return new Offset<FBS.DataProducer.GetStatsResponse>(o);
        }
        public GetStatsResponseT UnPack()
        {
            var _o = new GetStatsResponseT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(GetStatsResponseT _o)
        {
            _o.Timestamp = this.Timestamp;
            _o.Label = this.Label;
            _o.Protocol = this.Protocol;
            _o.MessagesReceived = this.MessagesReceived;
            _o.BytesReceived = this.BytesReceived;
            _o.BufferedAmount = this.BufferedAmount;
        }
        public static Offset<FBS.DataProducer.GetStatsResponse> Pack(FlatBufferBuilder builder, GetStatsResponseT _o)
        {
            if(_o == null)
                return default(Offset<FBS.DataProducer.GetStatsResponse>);
            var _label = _o.Label == null ? default(StringOffset) : builder.CreateString(_o.Label);
            var _protocol = _o.Protocol == null ? default(StringOffset) : builder.CreateString(_o.Protocol);
            return CreateGetStatsResponse(
              builder,
              _o.Timestamp,
              _label,
              _protocol,
              _o.MessagesReceived,
              _o.BytesReceived,
              _o.BufferedAmount);
        }
    }

    public class GetStatsResponseT
    {
        public ulong Timestamp { get; set; }

        public string Label { get; set; }

        public string Protocol { get; set; }

        public ulong MessagesReceived { get; set; }

        public ulong BytesReceived { get; set; }

        public uint BufferedAmount { get; set; }

        public GetStatsResponseT()
        {
            this.Timestamp = 0;
            this.Label = null;
            this.Protocol = null;
            this.MessagesReceived = 0;
            this.BytesReceived = 0;
            this.BufferedAmount = 0;
        }
    }


    static public class GetStatsResponseVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyField(tablePos, 4 /*Timestamp*/, 8 /*ulong*/, 8, false)
              && verifier.VerifyString(tablePos, 6 /*Label*/, true)
              && verifier.VerifyString(tablePos, 8 /*Protocol*/, true)
              && verifier.VerifyField(tablePos, 10 /*MessagesReceived*/, 8 /*ulong*/, 8, false)
              && verifier.VerifyField(tablePos, 12 /*BytesReceived*/, 8 /*ulong*/, 8, false)
              && verifier.VerifyField(tablePos, 14 /*BufferedAmount*/, 4 /*uint*/, 4, false)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
