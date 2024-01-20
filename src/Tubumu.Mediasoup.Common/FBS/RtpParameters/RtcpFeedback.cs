// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using Google.FlatBuffers;

namespace FBS.RtpParameters
{
    public struct RtcpFeedback : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static RtcpFeedback GetRootAsRtcpFeedback(ByteBuffer _bb) { return GetRootAsRtcpFeedback(_bb, new RtcpFeedback()); }
        public static RtcpFeedback GetRootAsRtcpFeedback(ByteBuffer _bb, RtcpFeedback obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public RtcpFeedback __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string Type { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetTypeBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetTypeBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetTypeArray() { return __p.__vector_as_array<byte>(4); }
        public string Parameter { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetParameterBytes() { return __p.__vector_as_span<byte>(6, 1); }
#else
        public ArraySegment<byte>? GetParameterBytes() { return __p.__vector_as_arraysegment(6); }
#endif
        public byte[] GetParameterArray() { return __p.__vector_as_array<byte>(6); }

        public static Offset<FBS.RtpParameters.RtcpFeedback> CreateRtcpFeedback(FlatBufferBuilder builder,
            StringOffset typeOffset = default(StringOffset),
            StringOffset parameterOffset = default(StringOffset))
        {
            builder.StartTable(2);
            RtcpFeedback.AddParameter(builder, parameterOffset);
            RtcpFeedback.AddType(builder, typeOffset);
            return RtcpFeedback.EndRtcpFeedback(builder);
        }

        public static void StartRtcpFeedback(FlatBufferBuilder builder) { builder.StartTable(2); }
        public static void AddType(FlatBufferBuilder builder, StringOffset typeOffset) { builder.AddOffset(0, typeOffset.Value, 0); }
        public static void AddParameter(FlatBufferBuilder builder, StringOffset parameterOffset) { builder.AddOffset(1, parameterOffset.Value, 0); }
        public static Offset<FBS.RtpParameters.RtcpFeedback> EndRtcpFeedback(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // type
            return new Offset<FBS.RtpParameters.RtcpFeedback>(o);
        }
        public RtcpFeedbackT UnPack()
        {
            var _o = new RtcpFeedbackT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(RtcpFeedbackT _o)
        {
            _o.Type = this.Type;
            _o.Parameter = this.Parameter;
        }
        public static Offset<FBS.RtpParameters.RtcpFeedback> Pack(FlatBufferBuilder builder, RtcpFeedbackT _o)
        {
            if(_o == null)
                return default(Offset<FBS.RtpParameters.RtcpFeedback>);
            var _type = _o.Type == null ? default(StringOffset) : builder.CreateString(_o.Type);
            var _parameter = _o.Parameter == null ? default(StringOffset) : builder.CreateString(_o.Parameter);
            return CreateRtcpFeedback(
              builder,
              _type,
              _parameter);
        }
    }
}
