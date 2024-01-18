// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using System.Collections.Generic;
using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.Transport
{
    public struct ConsumeRequest : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static ConsumeRequest GetRootAsConsumeRequest(ByteBuffer _bb) { return GetRootAsConsumeRequest(_bb, new ConsumeRequest()); }
        public static ConsumeRequest GetRootAsConsumeRequest(ByteBuffer _bb, ConsumeRequest obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public ConsumeRequest __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string ConsumerId { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetConsumerIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetConsumerIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetConsumerIdArray() { return __p.__vector_as_array<byte>(4); }
        public string ProducerId { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetProducerIdBytes() { return __p.__vector_as_span<byte>(6, 1); }
#else
        public ArraySegment<byte>? GetProducerIdBytes() { return __p.__vector_as_arraysegment(6); }
#endif
        public byte[] GetProducerIdArray() { return __p.__vector_as_array<byte>(6); }
        public FBS.RtpParameters.MediaKind Kind { get { int o = __p.__offset(8); return o != 0 ? (FBS.RtpParameters.MediaKind)__p.bb.Get(o + __p.bb_pos) : FBS.RtpParameters.MediaKind.AUDIO; } }
        public FBS.RtpParameters.RtpParameters? RtpParameters { get { int o = __p.__offset(10); return o != 0 ? (FBS.RtpParameters.RtpParameters?)(new FBS.RtpParameters.RtpParameters()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public FBS.RtpParameters.Type Type { get { int o = __p.__offset(12); return o != 0 ? (FBS.RtpParameters.Type)__p.bb.Get(o + __p.bb_pos) : FBS.RtpParameters.Type.SIMPLE; } }
        public FBS.RtpParameters.RtpEncodingParameters? ConsumableRtpEncodings(int j) { int o = __p.__offset(14); return o != 0 ? (FBS.RtpParameters.RtpEncodingParameters?)(new FBS.RtpParameters.RtpEncodingParameters()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int ConsumableRtpEncodingsLength { get { int o = __p.__offset(14); return o != 0 ? __p.__vector_len(o) : 0; } }
        public bool Paused { get { int o = __p.__offset(16); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }
        public FBS.Consumer.ConsumerLayers? PreferredLayers { get { int o = __p.__offset(18); return o != 0 ? (FBS.Consumer.ConsumerLayers?)(new FBS.Consumer.ConsumerLayers()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public bool IgnoreDtx { get { int o = __p.__offset(20); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }

        public static Offset<FBS.Transport.ConsumeRequest> CreateConsumeRequest(FlatBufferBuilder builder,
            StringOffset consumer_idOffset = default(StringOffset),
            StringOffset producer_idOffset = default(StringOffset),
            FBS.RtpParameters.MediaKind kind = FBS.RtpParameters.MediaKind.AUDIO,
            Offset<FBS.RtpParameters.RtpParameters> rtp_parametersOffset = default(Offset<FBS.RtpParameters.RtpParameters>),
            FBS.RtpParameters.Type type = FBS.RtpParameters.Type.SIMPLE,
            VectorOffset consumable_rtp_encodingsOffset = default(VectorOffset),
            bool paused = false,
            Offset<FBS.Consumer.ConsumerLayers> preferred_layersOffset = default(Offset<FBS.Consumer.ConsumerLayers>),
            bool ignore_dtx = false)
        {
            builder.StartTable(9);
            ConsumeRequest.AddPreferredLayers(builder, preferred_layersOffset);
            ConsumeRequest.AddConsumableRtpEncodings(builder, consumable_rtp_encodingsOffset);
            ConsumeRequest.AddRtpParameters(builder, rtp_parametersOffset);
            ConsumeRequest.AddProducerId(builder, producer_idOffset);
            ConsumeRequest.AddConsumerId(builder, consumer_idOffset);
            ConsumeRequest.AddIgnoreDtx(builder, ignore_dtx);
            ConsumeRequest.AddPaused(builder, paused);
            ConsumeRequest.AddType(builder, type);
            ConsumeRequest.AddKind(builder, kind);
            return ConsumeRequest.EndConsumeRequest(builder);
        }

        public static void StartConsumeRequest(FlatBufferBuilder builder) { builder.StartTable(9); }
        public static void AddConsumerId(FlatBufferBuilder builder, StringOffset consumerIdOffset) { builder.AddOffset(0, consumerIdOffset.Value, 0); }
        public static void AddProducerId(FlatBufferBuilder builder, StringOffset producerIdOffset) { builder.AddOffset(1, producerIdOffset.Value, 0); }
        public static void AddKind(FlatBufferBuilder builder, FBS.RtpParameters.MediaKind kind) { builder.AddByte(2, (byte)kind, 0); }
        public static void AddRtpParameters(FlatBufferBuilder builder, Offset<FBS.RtpParameters.RtpParameters> rtpParametersOffset) { builder.AddOffset(3, rtpParametersOffset.Value, 0); }
        public static void AddType(FlatBufferBuilder builder, FBS.RtpParameters.Type type) { builder.AddByte(4, (byte)type, 0); }
        public static void AddConsumableRtpEncodings(FlatBufferBuilder builder, VectorOffset consumableRtpEncodingsOffset) { builder.AddOffset(5, consumableRtpEncodingsOffset.Value, 0); }
        public static VectorOffset CreateConsumableRtpEncodingsVector(FlatBufferBuilder builder, Offset<FBS.RtpParameters.RtpEncodingParameters>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateConsumableRtpEncodingsVectorBlock(FlatBufferBuilder builder, Offset<FBS.RtpParameters.RtpEncodingParameters>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateConsumableRtpEncodingsVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.RtpParameters.RtpEncodingParameters>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateConsumableRtpEncodingsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.RtpParameters.RtpEncodingParameters>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartConsumableRtpEncodingsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddPaused(FlatBufferBuilder builder, bool paused) { builder.AddBool(6, paused, false); }
        public static void AddPreferredLayers(FlatBufferBuilder builder, Offset<FBS.Consumer.ConsumerLayers> preferredLayersOffset) { builder.AddOffset(7, preferredLayersOffset.Value, 0); }
        public static void AddIgnoreDtx(FlatBufferBuilder builder, bool ignoreDtx) { builder.AddBool(8, ignoreDtx, false); }
        public static Offset<FBS.Transport.ConsumeRequest> EndConsumeRequest(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // consumer_id
            builder.Required(o, 6);  // producer_id
            builder.Required(o, 10);  // rtp_parameters
            builder.Required(o, 14);  // consumable_rtp_encodings
            return new Offset<FBS.Transport.ConsumeRequest>(o);
        }
        public ConsumeRequestT UnPack()
        {
            var _o = new ConsumeRequestT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(ConsumeRequestT _o)
        {
            _o.ConsumerId = this.ConsumerId;
            _o.ProducerId = this.ProducerId;
            _o.Kind = this.Kind;
            _o.RtpParameters = this.RtpParameters.HasValue ? this.RtpParameters.Value.UnPack() : null;
            _o.Type = this.Type;
            _o.ConsumableRtpEncodings = new List<FBS.RtpParameters.RtpEncodingParametersT>();
            for(var _j = 0; _j < this.ConsumableRtpEncodingsLength; ++_j)
            { _o.ConsumableRtpEncodings.Add(this.ConsumableRtpEncodings(_j).HasValue ? this.ConsumableRtpEncodings(_j).Value.UnPack() : null); }
            _o.Paused = this.Paused;
            _o.PreferredLayers = this.PreferredLayers.HasValue ? this.PreferredLayers.Value.UnPack() : null;
            _o.IgnoreDtx = this.IgnoreDtx;
        }
        public static Offset<FBS.Transport.ConsumeRequest> Pack(FlatBufferBuilder builder, ConsumeRequestT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Transport.ConsumeRequest>);
            var _consumer_id = _o.ConsumerId == null ? default(StringOffset) : builder.CreateString(_o.ConsumerId);
            var _producer_id = _o.ProducerId == null ? default(StringOffset) : builder.CreateString(_o.ProducerId);
            var _rtp_parameters = _o.RtpParameters == null ? default(Offset<FBS.RtpParameters.RtpParameters>) : FBS.RtpParameters.RtpParameters.Pack(builder, _o.RtpParameters);
            var _consumable_rtp_encodings = default(VectorOffset);
            if(_o.ConsumableRtpEncodings != null)
            {
                var __consumable_rtp_encodings = new Offset<FBS.RtpParameters.RtpEncodingParameters>[_o.ConsumableRtpEncodings.Count];
                for(var _j = 0; _j < __consumable_rtp_encodings.Length; ++_j)
                { __consumable_rtp_encodings[_j] = FBS.RtpParameters.RtpEncodingParameters.Pack(builder, _o.ConsumableRtpEncodings[_j]); }
                _consumable_rtp_encodings = CreateConsumableRtpEncodingsVector(builder, __consumable_rtp_encodings);
            }
            var _preferred_layers = _o.PreferredLayers == null ? default(Offset<FBS.Consumer.ConsumerLayers>) : FBS.Consumer.ConsumerLayers.Pack(builder, _o.PreferredLayers);
            return CreateConsumeRequest(
              builder,
              _consumer_id,
              _producer_id,
              _o.Kind,
              _rtp_parameters,
              _o.Type,
              _consumable_rtp_encodings,
              _o.Paused,
              _preferred_layers,
              _o.IgnoreDtx);
        }
    }

    public class ConsumeRequestT
    {
        [JsonPropertyName("consumer_id")]
        public string ConsumerId { get; set; }
        [JsonPropertyName("producer_id")]
        public string ProducerId { get; set; }
        [JsonPropertyName("kind")]
        public FBS.RtpParameters.MediaKind Kind { get; set; }
        [JsonPropertyName("rtp_parameters")]
        public FBS.RtpParameters.RtpParametersT RtpParameters { get; set; }
        [JsonPropertyName("type")]
        public FBS.RtpParameters.Type Type { get; set; }
        [JsonPropertyName("consumable_rtp_encodings")]
        public List<FBS.RtpParameters.RtpEncodingParametersT> ConsumableRtpEncodings { get; set; }
        [JsonPropertyName("paused")]
        public bool Paused { get; set; }
        [JsonPropertyName("preferred_layers")]
        public FBS.Consumer.ConsumerLayersT PreferredLayers { get; set; }
        [JsonPropertyName("ignore_dtx")]
        public bool IgnoreDtx { get; set; }

        public ConsumeRequestT()
        {
            this.ConsumerId = null;
            this.ProducerId = null;
            this.Kind = FBS.RtpParameters.MediaKind.AUDIO;
            this.RtpParameters = null;
            this.Type = FBS.RtpParameters.Type.SIMPLE;
            this.ConsumableRtpEncodings = null;
            this.Paused = false;
            this.PreferredLayers = null;
            this.IgnoreDtx = false;
        }
    }
}