// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.RtpParameters
{
    public struct Parameter : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static Parameter GetRootAsParameter(ByteBuffer _bb) { return GetRootAsParameter(_bb, new Parameter()); }
        public static Parameter GetRootAsParameter(ByteBuffer _bb, Parameter obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public Parameter __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string Name { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetNameBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetNameArray() { return __p.__vector_as_array<byte>(4); }
        public FBS.RtpParameters.Value ValueType { get { int o = __p.__offset(6); return o != 0 ? (FBS.RtpParameters.Value)__p.bb.Get(o + __p.bb_pos) : FBS.RtpParameters.Value.NONE; } }
        public TTable? Value<TTable>() where TTable : struct, IFlatbufferObject { int o = __p.__offset(8); return o != 0 ? (TTable?)__p.__union<TTable>(o + __p.bb_pos) : null; }
        public FBS.RtpParameters.Boolean ValueAsBoolean() { return Value<FBS.RtpParameters.Boolean>().Value; }
        public FBS.RtpParameters.Integer32 ValueAsInteger32() { return Value<FBS.RtpParameters.Integer32>().Value; }
        public FBS.RtpParameters.Double ValueAsDouble() { return Value<FBS.RtpParameters.Double>().Value; }
        public FBS.RtpParameters.String ValueAsString() { return Value<FBS.RtpParameters.String>().Value; }
        public FBS.RtpParameters.Integer32Array ValueAsInteger32Array() { return Value<FBS.RtpParameters.Integer32Array>().Value; }

        public static Offset<FBS.RtpParameters.Parameter> CreateParameter(FlatBufferBuilder builder,
            StringOffset nameOffset = default(StringOffset),
            FBS.RtpParameters.Value value_type = FBS.RtpParameters.Value.NONE,
            int valueOffset = 0)
        {
            builder.StartTable(3);
            Parameter.AddValue(builder, valueOffset);
            Parameter.AddName(builder, nameOffset);
            Parameter.AddValueType(builder, value_type);
            return Parameter.EndParameter(builder);
        }

        public static void StartParameter(FlatBufferBuilder builder) { builder.StartTable(3); }
        public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
        public static void AddValueType(FlatBufferBuilder builder, FBS.RtpParameters.Value valueType) { builder.AddByte(1, (byte)valueType, 0); }
        public static void AddValue(FlatBufferBuilder builder, int valueOffset) { builder.AddOffset(2, valueOffset, 0); }
        public static Offset<FBS.RtpParameters.Parameter> EndParameter(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // name
            builder.Required(o, 8);  // value
            return new Offset<FBS.RtpParameters.Parameter>(o);
        }
        public ParameterT UnPack()
        {
            var _o = new ParameterT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(ParameterT _o)
        {
            _o.Name = this.Name;
            _o.Value = new FBS.RtpParameters.ValueUnion();
            _o.Value.Type = this.ValueType;
            switch(this.ValueType)
            {
                default:
                    break;
                case FBS.RtpParameters.Value.Boolean:
                    _o.Value.Value_ = this.Value<FBS.RtpParameters.Boolean>().HasValue ? this.Value<FBS.RtpParameters.Boolean>().Value.UnPack() : null;
                    break;
                case FBS.RtpParameters.Value.Integer32:
                    _o.Value.Value_ = this.Value<FBS.RtpParameters.Integer32>().HasValue ? this.Value<FBS.RtpParameters.Integer32>().Value.UnPack() : null;
                    break;
                case FBS.RtpParameters.Value.Double:
                    _o.Value.Value_ = this.Value<FBS.RtpParameters.Double>().HasValue ? this.Value<FBS.RtpParameters.Double>().Value.UnPack() : null;
                    break;
                case FBS.RtpParameters.Value.String:
                    _o.Value.Value_ = this.Value<FBS.RtpParameters.String>().HasValue ? this.Value<FBS.RtpParameters.String>().Value.UnPack() : null;
                    break;
                case FBS.RtpParameters.Value.Integer32Array:
                    _o.Value.Value_ = this.Value<FBS.RtpParameters.Integer32Array>().HasValue ? this.Value<FBS.RtpParameters.Integer32Array>().Value.UnPack() : null;
                    break;
            }
        }
        public static Offset<FBS.RtpParameters.Parameter> Pack(FlatBufferBuilder builder, ParameterT _o)
        {
            if(_o == null)
                return default(Offset<FBS.RtpParameters.Parameter>);
            var _name = _o.Name == null ? default(StringOffset) : builder.CreateString(_o.Name);
            var _value_type = _o.Value == null ? FBS.RtpParameters.Value.NONE : _o.Value.Type;
            var _value = _o.Value == null ? 0 : FBS.RtpParameters.ValueUnion.Pack(builder, _o.Value);
            return CreateParameter(
              builder,
              _name,
              _value_type,
              _value);
        }
    }

    public class ParameterT
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("value_type")]
        private FBS.RtpParameters.Value ValueType
        {
            get
            {
                return this.Value != null ? this.Value.Type : FBS.RtpParameters.Value.NONE;
            }
            set
            {
                this.Value = new FBS.RtpParameters.ValueUnion();
                this.Value.Type = value;
            }
        }
        [JsonPropertyName("value")]
        [JsonConverter(typeof(FBS.RtpParameters.ValueUnion_JsonConverter))]
        public FBS.RtpParameters.ValueUnion Value { get; set; }

        public ParameterT()
        {
            this.Name = null;
            this.Value = null;
        }
    }
}
