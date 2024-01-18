// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.RtxStream
{
    public struct Params : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static Params GetRootAsParams(ByteBuffer _bb) { return GetRootAsParams(_bb, new Params()); }
        public static Params GetRootAsParams(ByteBuffer _bb, Params obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public Params __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public uint Ssrc { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public byte PayloadType { get { int o = __p.__offset(6); return o != 0 ? __p.bb.Get(o + __p.bb_pos) : (byte)0; } }
        public string MimeType { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetMimeTypeBytes() { return __p.__vector_as_span<byte>(8, 1); }
#else
        public ArraySegment<byte>? GetMimeTypeBytes() { return __p.__vector_as_arraysegment(8); }
#endif
        public byte[] GetMimeTypeArray() { return __p.__vector_as_array<byte>(8); }
        public uint ClockRate { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public string Rrid { get { int o = __p.__offset(12); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetRridBytes() { return __p.__vector_as_span<byte>(12, 1); }
#else
        public ArraySegment<byte>? GetRridBytes() { return __p.__vector_as_arraysegment(12); }
#endif
        public byte[] GetRridArray() { return __p.__vector_as_array<byte>(12); }
        public string Cname { get { int o = __p.__offset(14); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetCnameBytes() { return __p.__vector_as_span<byte>(14, 1); }
#else
        public ArraySegment<byte>? GetCnameBytes() { return __p.__vector_as_arraysegment(14); }
#endif
        public byte[] GetCnameArray() { return __p.__vector_as_array<byte>(14); }

        public static Offset<FBS.RtxStream.Params> CreateParams(FlatBufferBuilder builder,
            uint ssrc = 0,
            byte payload_type = 0,
            StringOffset mime_typeOffset = default(StringOffset),
            uint clock_rate = 0,
            StringOffset rridOffset = default(StringOffset),
            StringOffset cnameOffset = default(StringOffset))
        {
            builder.StartTable(6);
            Params.AddCname(builder, cnameOffset);
            Params.AddRrid(builder, rridOffset);
            Params.AddClockRate(builder, clock_rate);
            Params.AddMimeType(builder, mime_typeOffset);
            Params.AddSsrc(builder, ssrc);
            Params.AddPayloadType(builder, payload_type);
            return Params.EndParams(builder);
        }

        public static void StartParams(FlatBufferBuilder builder) { builder.StartTable(6); }
        public static void AddSsrc(FlatBufferBuilder builder, uint ssrc) { builder.AddUint(0, ssrc, 0); }
        public static void AddPayloadType(FlatBufferBuilder builder, byte payloadType) { builder.AddByte(1, payloadType, 0); }
        public static void AddMimeType(FlatBufferBuilder builder, StringOffset mimeTypeOffset) { builder.AddOffset(2, mimeTypeOffset.Value, 0); }
        public static void AddClockRate(FlatBufferBuilder builder, uint clockRate) { builder.AddUint(3, clockRate, 0); }
        public static void AddRrid(FlatBufferBuilder builder, StringOffset rridOffset) { builder.AddOffset(4, rridOffset.Value, 0); }
        public static void AddCname(FlatBufferBuilder builder, StringOffset cnameOffset) { builder.AddOffset(5, cnameOffset.Value, 0); }
        public static Offset<FBS.RtxStream.Params> EndParams(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 8);  // mime_type
            builder.Required(o, 14);  // cname
            return new Offset<FBS.RtxStream.Params>(o);
        }
        public ParamsT UnPack()
        {
            var _o = new ParamsT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(ParamsT _o)
        {
            _o.Ssrc = this.Ssrc;
            _o.PayloadType = this.PayloadType;
            _o.MimeType = this.MimeType;
            _o.ClockRate = this.ClockRate;
            _o.Rrid = this.Rrid;
            _o.Cname = this.Cname;
        }
        public static Offset<FBS.RtxStream.Params> Pack(FlatBufferBuilder builder, ParamsT _o)
        {
            if(_o == null)
                return default(Offset<FBS.RtxStream.Params>);
            var _mime_type = _o.MimeType == null ? default(StringOffset) : builder.CreateString(_o.MimeType);
            var _rrid = _o.Rrid == null ? default(StringOffset) : builder.CreateString(_o.Rrid);
            var _cname = _o.Cname == null ? default(StringOffset) : builder.CreateString(_o.Cname);
            return CreateParams(
              builder,
              _o.Ssrc,
              _o.PayloadType,
              _mime_type,
              _o.ClockRate,
              _rrid,
              _cname);
        }
    }

    public class ParamsT
    {
        [JsonPropertyName("ssrc")]
        public uint Ssrc { get; set; }
        [JsonPropertyName("payload_type")]
        public byte PayloadType { get; set; }
        [JsonPropertyName("mime_type")]
        public string MimeType { get; set; }
        [JsonPropertyName("clock_rate")]
        public uint ClockRate { get; set; }
        [JsonPropertyName("rrid")]
        public string Rrid { get; set; }
        [JsonPropertyName("cname")]
        public string Cname { get; set; }

        public ParamsT()
        {
            this.Ssrc = 0;
            this.PayloadType = 0;
            this.MimeType = null;
            this.ClockRate = 0;
            this.Rrid = null;
            this.Cname = null;
        }
    }
}
