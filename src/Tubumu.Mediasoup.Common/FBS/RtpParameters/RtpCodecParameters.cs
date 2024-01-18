// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.RtpParameters
{
    public struct RtpCodecParameters : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static RtpCodecParameters GetRootAsRtpCodecParameters(ByteBuffer _bb) { return GetRootAsRtpCodecParameters(_bb, new RtpCodecParameters()); }
        public static RtpCodecParameters GetRootAsRtpCodecParameters(ByteBuffer _bb, RtpCodecParameters obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public RtpCodecParameters __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string MimeType { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetMimeTypeBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetMimeTypeBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetMimeTypeArray() { return __p.__vector_as_array<byte>(4); }
        public byte PayloadType { get { int o = __p.__offset(6); return o != 0 ? __p.bb.Get(o + __p.bb_pos) : (byte)0; } }
        public uint ClockRate { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public byte? Channels { get { int o = __p.__offset(10); return o != 0 ? __p.bb.Get(o + __p.bb_pos) : (byte?)null; } }
        public FBS.RtpParameters.Parameter? Parameters(int j) { int o = __p.__offset(12); return o != 0 ? (FBS.RtpParameters.Parameter?)(new FBS.RtpParameters.Parameter()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int ParametersLength { get { int o = __p.__offset(12); return o != 0 ? __p.__vector_len(o) : 0; } }
        public FBS.RtpParameters.RtcpFeedback? RtcpFeedback(int j) { int o = __p.__offset(14); return o != 0 ? (FBS.RtpParameters.RtcpFeedback?)(new FBS.RtpParameters.RtcpFeedback()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int RtcpFeedbackLength { get { int o = __p.__offset(14); return o != 0 ? __p.__vector_len(o) : 0; } }

        public static Offset<FBS.RtpParameters.RtpCodecParameters> CreateRtpCodecParameters(FlatBufferBuilder builder,
            StringOffset mime_typeOffset = default(StringOffset),
            byte payload_type = 0,
            uint clock_rate = 0,
            byte? channels = null,
            VectorOffset parametersOffset = default(VectorOffset),
            VectorOffset rtcp_feedbackOffset = default(VectorOffset))
        {
            builder.StartTable(6);
            RtpCodecParameters.AddRtcpFeedback(builder, rtcp_feedbackOffset);
            RtpCodecParameters.AddParameters(builder, parametersOffset);
            RtpCodecParameters.AddClockRate(builder, clock_rate);
            RtpCodecParameters.AddMimeType(builder, mime_typeOffset);
            RtpCodecParameters.AddChannels(builder, channels);
            RtpCodecParameters.AddPayloadType(builder, payload_type);
            return RtpCodecParameters.EndRtpCodecParameters(builder);
        }

        public static void StartRtpCodecParameters(FlatBufferBuilder builder) { builder.StartTable(6); }
        public static void AddMimeType(FlatBufferBuilder builder, StringOffset mimeTypeOffset) { builder.AddOffset(0, mimeTypeOffset.Value, 0); }
        public static void AddPayloadType(FlatBufferBuilder builder, byte payloadType) { builder.AddByte(1, payloadType, 0); }
        public static void AddClockRate(FlatBufferBuilder builder, uint clockRate) { builder.AddUint(2, clockRate, 0); }
        public static void AddChannels(FlatBufferBuilder builder, byte? channels) { builder.AddByte(3, channels); }
        public static void AddParameters(FlatBufferBuilder builder, VectorOffset parametersOffset) { builder.AddOffset(4, parametersOffset.Value, 0); }
        public static VectorOffset CreateParametersVector(FlatBufferBuilder builder, Offset<FBS.RtpParameters.Parameter>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateParametersVectorBlock(FlatBufferBuilder builder, Offset<FBS.RtpParameters.Parameter>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateParametersVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.RtpParameters.Parameter>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateParametersVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.RtpParameters.Parameter>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartParametersVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddRtcpFeedback(FlatBufferBuilder builder, VectorOffset rtcpFeedbackOffset) { builder.AddOffset(5, rtcpFeedbackOffset.Value, 0); }
        public static VectorOffset CreateRtcpFeedbackVector(FlatBufferBuilder builder, Offset<FBS.RtpParameters.RtcpFeedback>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateRtcpFeedbackVectorBlock(FlatBufferBuilder builder, Offset<FBS.RtpParameters.RtcpFeedback>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateRtcpFeedbackVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.RtpParameters.RtcpFeedback>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateRtcpFeedbackVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.RtpParameters.RtcpFeedback>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartRtcpFeedbackVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static Offset<FBS.RtpParameters.RtpCodecParameters> EndRtpCodecParameters(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // mime_type
            return new Offset<FBS.RtpParameters.RtpCodecParameters>(o);
        }
        public RtpCodecParametersT UnPack()
        {
            var _o = new RtpCodecParametersT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(RtpCodecParametersT _o)
        {
            _o.MimeType = this.MimeType;
            _o.PayloadType = this.PayloadType;
            _o.ClockRate = this.ClockRate;
            _o.Channels = this.Channels;
            _o.Parameters = new List<FBS.RtpParameters.ParameterT>();
            for(var _j = 0; _j < this.ParametersLength; ++_j)
            { _o.Parameters.Add(this.Parameters(_j).HasValue ? this.Parameters(_j).Value.UnPack() : null); }
            _o.RtcpFeedback = new List<FBS.RtpParameters.RtcpFeedbackT>();
            for(var _j = 0; _j < this.RtcpFeedbackLength; ++_j)
            { _o.RtcpFeedback.Add(this.RtcpFeedback(_j).HasValue ? this.RtcpFeedback(_j).Value.UnPack() : null); }
        }
        public static Offset<FBS.RtpParameters.RtpCodecParameters> Pack(FlatBufferBuilder builder, RtpCodecParametersT _o)
        {
            if(_o == null)
                return default(Offset<FBS.RtpParameters.RtpCodecParameters>);
            var _mime_type = _o.MimeType == null ? default(StringOffset) : builder.CreateString(_o.MimeType);
            var _parameters = default(VectorOffset);
            if(_o.Parameters != null)
            {
                var __parameters = new Offset<FBS.RtpParameters.Parameter>[_o.Parameters.Count];
                for(var _j = 0; _j < __parameters.Length; ++_j)
                { __parameters[_j] = FBS.RtpParameters.Parameter.Pack(builder, _o.Parameters[_j]); }
                _parameters = CreateParametersVector(builder, __parameters);
            }
            var _rtcp_feedback = default(VectorOffset);
            if(_o.RtcpFeedback != null)
            {
                var __rtcp_feedback = new Offset<FBS.RtpParameters.RtcpFeedback>[_o.RtcpFeedback.Count];
                for(var _j = 0; _j < __rtcp_feedback.Length; ++_j)
                { __rtcp_feedback[_j] = FBS.RtpParameters.RtcpFeedback.Pack(builder, _o.RtcpFeedback[_j]); }
                _rtcp_feedback = CreateRtcpFeedbackVector(builder, __rtcp_feedback);
            }
            return CreateRtpCodecParameters(
              builder,
              _mime_type,
              _o.PayloadType,
              _o.ClockRate,
              _o.Channels,
              _parameters,
              _rtcp_feedback);
        }
    }

    public class RtpCodecParametersT
    {
        [JsonPropertyName("mime_type")]
        public string MimeType { get; set; }
        [JsonPropertyName("payload_type")]
        public byte PayloadType { get; set; }
        [JsonPropertyName("clock_rate")]
        public uint ClockRate { get; set; }
        [JsonPropertyName("channels")]
        public byte? Channels { get; set; }
        [JsonPropertyName("parameters")]
        public List<FBS.RtpParameters.ParameterT> Parameters { get; set; }
        [JsonPropertyName("rtcp_feedback")]
        public List<FBS.RtpParameters.RtcpFeedbackT> RtcpFeedback { get; set; }

        public RtpCodecParametersT()
        {
            this.MimeType = null;
            this.PayloadType = 0;
            this.ClockRate = 0;
            this.Channels = null;
            this.Parameters = null;
            this.RtcpFeedback = null;
        }
    }
}