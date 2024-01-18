// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.Transport
{
    public struct ProduceDataRequest : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static ProduceDataRequest GetRootAsProduceDataRequest(ByteBuffer _bb) { return GetRootAsProduceDataRequest(_bb, new ProduceDataRequest()); }
        public static ProduceDataRequest GetRootAsProduceDataRequest(ByteBuffer _bb, ProduceDataRequest obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public ProduceDataRequest __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string DataProducerId { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetDataProducerIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetDataProducerIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetDataProducerIdArray() { return __p.__vector_as_array<byte>(4); }
        public FBS.DataProducer.Type Type { get { int o = __p.__offset(6); return o != 0 ? (FBS.DataProducer.Type)__p.bb.Get(o + __p.bb_pos) : FBS.DataProducer.Type.SCTP; } }
        public FBS.SctpParameters.SctpStreamParameters? SctpStreamParameters { get { int o = __p.__offset(8); return o != 0 ? (FBS.SctpParameters.SctpStreamParameters?)(new FBS.SctpParameters.SctpStreamParameters()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public string Label { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetLabelBytes() { return __p.__vector_as_span<byte>(10, 1); }
#else
        public ArraySegment<byte>? GetLabelBytes() { return __p.__vector_as_arraysegment(10); }
#endif
        public byte[] GetLabelArray() { return __p.__vector_as_array<byte>(10); }
        public string Protocol { get { int o = __p.__offset(12); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetProtocolBytes() { return __p.__vector_as_span<byte>(12, 1); }
#else
        public ArraySegment<byte>? GetProtocolBytes() { return __p.__vector_as_arraysegment(12); }
#endif
        public byte[] GetProtocolArray() { return __p.__vector_as_array<byte>(12); }
        public bool Paused { get { int o = __p.__offset(14); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }

        public static Offset<FBS.Transport.ProduceDataRequest> CreateProduceDataRequest(FlatBufferBuilder builder,
            StringOffset data_producer_idOffset = default(StringOffset),
            FBS.DataProducer.Type type = FBS.DataProducer.Type.SCTP,
            Offset<FBS.SctpParameters.SctpStreamParameters> sctp_stream_parametersOffset = default(Offset<FBS.SctpParameters.SctpStreamParameters>),
            StringOffset labelOffset = default(StringOffset),
            StringOffset protocolOffset = default(StringOffset),
            bool paused = false)
        {
            builder.StartTable(6);
            ProduceDataRequest.AddProtocol(builder, protocolOffset);
            ProduceDataRequest.AddLabel(builder, labelOffset);
            ProduceDataRequest.AddSctpStreamParameters(builder, sctp_stream_parametersOffset);
            ProduceDataRequest.AddDataProducerId(builder, data_producer_idOffset);
            ProduceDataRequest.AddPaused(builder, paused);
            ProduceDataRequest.AddType(builder, type);
            return ProduceDataRequest.EndProduceDataRequest(builder);
        }

        public static void StartProduceDataRequest(FlatBufferBuilder builder) { builder.StartTable(6); }
        public static void AddDataProducerId(FlatBufferBuilder builder, StringOffset dataProducerIdOffset) { builder.AddOffset(0, dataProducerIdOffset.Value, 0); }
        public static void AddType(FlatBufferBuilder builder, FBS.DataProducer.Type type) { builder.AddByte(1, (byte)type, 0); }
        public static void AddSctpStreamParameters(FlatBufferBuilder builder, Offset<FBS.SctpParameters.SctpStreamParameters> sctpStreamParametersOffset) { builder.AddOffset(2, sctpStreamParametersOffset.Value, 0); }
        public static void AddLabel(FlatBufferBuilder builder, StringOffset labelOffset) { builder.AddOffset(3, labelOffset.Value, 0); }
        public static void AddProtocol(FlatBufferBuilder builder, StringOffset protocolOffset) { builder.AddOffset(4, protocolOffset.Value, 0); }
        public static void AddPaused(FlatBufferBuilder builder, bool paused) { builder.AddBool(5, paused, false); }
        public static Offset<FBS.Transport.ProduceDataRequest> EndProduceDataRequest(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // data_producer_id
            return new Offset<FBS.Transport.ProduceDataRequest>(o);
        }
        public ProduceDataRequestT UnPack()
        {
            var _o = new ProduceDataRequestT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(ProduceDataRequestT _o)
        {
            _o.DataProducerId = this.DataProducerId;
            _o.Type = this.Type;
            _o.SctpStreamParameters = this.SctpStreamParameters.HasValue ? this.SctpStreamParameters.Value.UnPack() : null;
            _o.Label = this.Label;
            _o.Protocol = this.Protocol;
            _o.Paused = this.Paused;
        }
        public static Offset<FBS.Transport.ProduceDataRequest> Pack(FlatBufferBuilder builder, ProduceDataRequestT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Transport.ProduceDataRequest>);
            var _data_producer_id = _o.DataProducerId == null ? default(StringOffset) : builder.CreateString(_o.DataProducerId);
            var _sctp_stream_parameters = _o.SctpStreamParameters == null ? default(Offset<FBS.SctpParameters.SctpStreamParameters>) : FBS.SctpParameters.SctpStreamParameters.Pack(builder, _o.SctpStreamParameters);
            var _label = _o.Label == null ? default(StringOffset) : builder.CreateString(_o.Label);
            var _protocol = _o.Protocol == null ? default(StringOffset) : builder.CreateString(_o.Protocol);
            return CreateProduceDataRequest(
              builder,
              _data_producer_id,
              _o.Type,
              _sctp_stream_parameters,
              _label,
              _protocol,
              _o.Paused);
        }
    }

    public class ProduceDataRequestT
    {
        [JsonPropertyName("data_producer_id")]
        public string DataProducerId { get; set; }
        [JsonPropertyName("type")]
        public FBS.DataProducer.Type Type { get; set; }
        [JsonPropertyName("sctp_stream_parameters")]
        public FBS.SctpParameters.SctpStreamParametersT SctpStreamParameters { get; set; }
        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }
        [JsonPropertyName("paused")]
        public bool Paused { get; set; }

        public ProduceDataRequestT()
        {
            this.DataProducerId = null;
            this.Type = FBS.DataProducer.Type.SCTP;
            this.SctpStreamParameters = null;
            this.Label = null;
            this.Protocol = null;
            this.Paused = false;
        }
    }
}