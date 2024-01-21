// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.Transport
{
    public struct ConsumeDataRequest : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static ConsumeDataRequest GetRootAsConsumeDataRequest(ByteBuffer _bb) { return GetRootAsConsumeDataRequest(_bb, new ConsumeDataRequest()); }
        public static ConsumeDataRequest GetRootAsConsumeDataRequest(ByteBuffer _bb, ConsumeDataRequest obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public ConsumeDataRequest __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string DataConsumerId { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetDataConsumerIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetDataConsumerIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetDataConsumerIdArray() { return __p.__vector_as_array<byte>(4); }
        public string DataProducerId { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetDataProducerIdBytes() { return __p.__vector_as_span<byte>(6, 1); }
#else
        public ArraySegment<byte>? GetDataProducerIdBytes() { return __p.__vector_as_arraysegment(6); }
#endif
        public byte[] GetDataProducerIdArray() { return __p.__vector_as_array<byte>(6); }
        public FBS.DataProducer.Type Type { get { int o = __p.__offset(8); return o != 0 ? (FBS.DataProducer.Type)__p.bb.Get(o + __p.bb_pos) : FBS.DataProducer.Type.SCTP; } }
        public FBS.SctpParameters.SctpStreamParameters? SctpStreamParameters { get { int o = __p.__offset(10); return o != 0 ? (FBS.SctpParameters.SctpStreamParameters?)(new FBS.SctpParameters.SctpStreamParameters()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public string Label { get { int o = __p.__offset(12); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetLabelBytes() { return __p.__vector_as_span<byte>(12, 1); }
#else
        public ArraySegment<byte>? GetLabelBytes() { return __p.__vector_as_arraysegment(12); }
#endif
        public byte[] GetLabelArray() { return __p.__vector_as_array<byte>(12); }
        public string Protocol { get { int o = __p.__offset(14); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetProtocolBytes() { return __p.__vector_as_span<byte>(14, 1); }
#else
        public ArraySegment<byte>? GetProtocolBytes() { return __p.__vector_as_arraysegment(14); }
#endif
        public byte[] GetProtocolArray() { return __p.__vector_as_array<byte>(14); }
        public bool Paused { get { int o = __p.__offset(16); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }
        public ushort Subchannels(int j) { int o = __p.__offset(18); return o != 0 ? __p.bb.GetUshort(__p.__vector(o) + j * 2) : (ushort)0; }
        public int SubchannelsLength { get { int o = __p.__offset(18); return o != 0 ? __p.__vector_len(o) : 0; } }
#if ENABLE_SPAN_T
  public Span<ushort> GetSubchannelsBytes() { return __p.__vector_as_span<ushort>(18, 2); }
#else
        public ArraySegment<byte>? GetSubchannelsBytes() { return __p.__vector_as_arraysegment(18); }
#endif
        public ushort[] GetSubchannelsArray() { return __p.__vector_as_array<ushort>(18); }

        public static Offset<FBS.Transport.ConsumeDataRequest> CreateConsumeDataRequest(FlatBufferBuilder builder,
            StringOffset data_consumer_idOffset = default(StringOffset),
            StringOffset data_producer_idOffset = default(StringOffset),
            FBS.DataProducer.Type type = FBS.DataProducer.Type.SCTP,
            Offset<FBS.SctpParameters.SctpStreamParameters> sctp_stream_parametersOffset = default(Offset<FBS.SctpParameters.SctpStreamParameters>),
            StringOffset labelOffset = default(StringOffset),
            StringOffset protocolOffset = default(StringOffset),
            bool paused = false,
            VectorOffset subchannelsOffset = default(VectorOffset))
        {
            builder.StartTable(8);
            ConsumeDataRequest.AddSubchannels(builder, subchannelsOffset);
            ConsumeDataRequest.AddProtocol(builder, protocolOffset);
            ConsumeDataRequest.AddLabel(builder, labelOffset);
            ConsumeDataRequest.AddSctpStreamParameters(builder, sctp_stream_parametersOffset);
            ConsumeDataRequest.AddDataProducerId(builder, data_producer_idOffset);
            ConsumeDataRequest.AddDataConsumerId(builder, data_consumer_idOffset);
            ConsumeDataRequest.AddPaused(builder, paused);
            ConsumeDataRequest.AddType(builder, type);
            return ConsumeDataRequest.EndConsumeDataRequest(builder);
        }

        public static void StartConsumeDataRequest(FlatBufferBuilder builder) { builder.StartTable(8); }
        public static void AddDataConsumerId(FlatBufferBuilder builder, StringOffset dataConsumerIdOffset) { builder.AddOffset(0, dataConsumerIdOffset.Value, 0); }
        public static void AddDataProducerId(FlatBufferBuilder builder, StringOffset dataProducerIdOffset) { builder.AddOffset(1, dataProducerIdOffset.Value, 0); }
        public static void AddType(FlatBufferBuilder builder, FBS.DataProducer.Type type) { builder.AddByte(2, (byte)type, 0); }
        public static void AddSctpStreamParameters(FlatBufferBuilder builder, Offset<FBS.SctpParameters.SctpStreamParameters> sctpStreamParametersOffset) { builder.AddOffset(3, sctpStreamParametersOffset.Value, 0); }
        public static void AddLabel(FlatBufferBuilder builder, StringOffset labelOffset) { builder.AddOffset(4, labelOffset.Value, 0); }
        public static void AddProtocol(FlatBufferBuilder builder, StringOffset protocolOffset) { builder.AddOffset(5, protocolOffset.Value, 0); }
        public static void AddPaused(FlatBufferBuilder builder, bool paused) { builder.AddBool(6, paused, false); }
        public static void AddSubchannels(FlatBufferBuilder builder, VectorOffset subchannelsOffset) { builder.AddOffset(7, subchannelsOffset.Value, 0); }
        public static VectorOffset CreateSubchannelsVector(FlatBufferBuilder builder, ushort[] data) { builder.StartVector(2, data.Length, 2); for(int i = data.Length - 1; i >= 0; i--) builder.AddUshort(data[i]); return builder.EndVector(); }
        public static VectorOffset CreateSubchannelsVectorBlock(FlatBufferBuilder builder, ushort[] data) { builder.StartVector(2, data.Length, 2); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateSubchannelsVectorBlock(FlatBufferBuilder builder, ArraySegment<ushort> data) { builder.StartVector(2, data.Count, 2); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateSubchannelsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<ushort>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartSubchannelsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(2, numElems, 2); }
        public static Offset<FBS.Transport.ConsumeDataRequest> EndConsumeDataRequest(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // data_consumer_id
            builder.Required(o, 6);  // data_producer_id
            return new Offset<FBS.Transport.ConsumeDataRequest>(o);
        }
        public ConsumeDataRequestT UnPack()
        {
            var _o = new ConsumeDataRequestT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(ConsumeDataRequestT _o)
        {
            _o.DataConsumerId = this.DataConsumerId;
            _o.DataProducerId = this.DataProducerId;
            _o.Type = this.Type;
            _o.SctpStreamParameters = this.SctpStreamParameters.HasValue ? this.SctpStreamParameters.Value.UnPack() : null;
            _o.Label = this.Label;
            _o.Protocol = this.Protocol;
            _o.Paused = this.Paused;
            _o.Subchannels = new List<ushort>();
            for(var _j = 0; _j < this.SubchannelsLength; ++_j)
            { _o.Subchannels.Add(this.Subchannels(_j)); }
        }
        public static Offset<FBS.Transport.ConsumeDataRequest> Pack(FlatBufferBuilder builder, ConsumeDataRequestT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Transport.ConsumeDataRequest>);
            var _data_consumer_id = _o.DataConsumerId == null ? default(StringOffset) : builder.CreateString(_o.DataConsumerId);
            var _data_producer_id = _o.DataProducerId == null ? default(StringOffset) : builder.CreateString(_o.DataProducerId);
            var _sctp_stream_parameters = _o.SctpStreamParameters == null ? default(Offset<FBS.SctpParameters.SctpStreamParameters>) : FBS.SctpParameters.SctpStreamParameters.Pack(builder, _o.SctpStreamParameters);
            var _label = _o.Label == null ? default(StringOffset) : builder.CreateString(_o.Label);
            var _protocol = _o.Protocol == null ? default(StringOffset) : builder.CreateString(_o.Protocol);
            var _subchannels = default(VectorOffset);
            if(_o.Subchannels != null)
            {
                var __subchannels = _o.Subchannels.ToArray();
                _subchannels = CreateSubchannelsVector(builder, __subchannels);
            }
            return CreateConsumeDataRequest(
              builder,
              _data_consumer_id,
              _data_producer_id,
              _o.Type,
              _sctp_stream_parameters,
              _label,
              _protocol,
              _o.Paused,
              _subchannels);
        }
    }
}
