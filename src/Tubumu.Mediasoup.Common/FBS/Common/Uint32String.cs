// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FBS.Common
{
    using System;
    using System.Text.Json.Serialization;
    using Google.FlatBuffers;

    public struct Uint32String : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static Uint32String GetRootAsUint32String(ByteBuffer _bb) { return GetRootAsUint32String(_bb, new Uint32String()); }
        public static Uint32String GetRootAsUint32String(ByteBuffer _bb, Uint32String obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public Uint32String __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public uint Key { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public string Value { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetValueBytes() { return __p.__vector_as_span<byte>(6, 1); }
#else
        public ArraySegment<byte>? GetValueBytes() { return __p.__vector_as_arraysegment(6); }
#endif
        public byte[] GetValueArray() { return __p.__vector_as_array<byte>(6); }

        public static Offset<FBS.Common.Uint32String> CreateUint32String(FlatBufferBuilder builder,
            uint key = 0,
            StringOffset valueOffset = default(StringOffset))
        {
            builder.StartTable(2);
            Uint32String.AddValue(builder, valueOffset);
            Uint32String.AddKey(builder, key);
            return Uint32String.EndUint32String(builder);
        }

        public static void StartUint32String(FlatBufferBuilder builder) { builder.StartTable(2); }
        public static void AddKey(FlatBufferBuilder builder, uint key) { builder.AddUint(0, key, 0); }
        public static void AddValue(FlatBufferBuilder builder, StringOffset valueOffset) { builder.AddOffset(1, valueOffset.Value, 0); }
        public static Offset<FBS.Common.Uint32String> EndUint32String(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 6);  // value
            return new Offset<FBS.Common.Uint32String>(o);
        }
        public Uint32StringT UnPack()
        {
            var _o = new Uint32StringT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(Uint32StringT _o)
        {
            _o.Key = this.Key;
            _o.Value = this.Value;
        }
        public static Offset<FBS.Common.Uint32String> Pack(FlatBufferBuilder builder, Uint32StringT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Common.Uint32String>);
            var _value = _o.Value == null ? default(StringOffset) : builder.CreateString(_o.Value);
            return CreateUint32String(
              builder,
              _o.Key,
              _value);
        }
    }

    public class Uint32StringT
    {
        [JsonPropertyName("key")]
        public uint Key { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }

        public Uint32StringT()
        {
            this.Key = 0;
            this.Value = null;
        }
    }
}
