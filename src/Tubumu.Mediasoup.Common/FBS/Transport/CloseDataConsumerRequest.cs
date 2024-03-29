// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.Transport
{
    public struct CloseDataConsumerRequest : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static CloseDataConsumerRequest GetRootAsCloseDataConsumerRequest(ByteBuffer _bb) { return GetRootAsCloseDataConsumerRequest(_bb, new CloseDataConsumerRequest()); }
        public static CloseDataConsumerRequest GetRootAsCloseDataConsumerRequest(ByteBuffer _bb, CloseDataConsumerRequest obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public CloseDataConsumerRequest __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string DataConsumerId { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetDataConsumerIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetDataConsumerIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetDataConsumerIdArray() { return __p.__vector_as_array<byte>(4); }

        public static Offset<FBS.Transport.CloseDataConsumerRequest> CreateCloseDataConsumerRequest(FlatBufferBuilder builder,
            StringOffset data_consumer_idOffset = default(StringOffset))
        {
            builder.StartTable(1);
            CloseDataConsumerRequest.AddDataConsumerId(builder, data_consumer_idOffset);
            return CloseDataConsumerRequest.EndCloseDataConsumerRequest(builder);
        }

        public static void StartCloseDataConsumerRequest(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddDataConsumerId(FlatBufferBuilder builder, StringOffset dataConsumerIdOffset) { builder.AddOffset(0, dataConsumerIdOffset.Value, 0); }
        public static Offset<FBS.Transport.CloseDataConsumerRequest> EndCloseDataConsumerRequest(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // data_consumer_id
            return new Offset<FBS.Transport.CloseDataConsumerRequest>(o);
        }
        public CloseDataConsumerRequestT UnPack()
        {
            var _o = new CloseDataConsumerRequestT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(CloseDataConsumerRequestT _o)
        {
            _o.DataConsumerId = this.DataConsumerId;
        }
        public static Offset<FBS.Transport.CloseDataConsumerRequest> Pack(FlatBufferBuilder builder, CloseDataConsumerRequestT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Transport.CloseDataConsumerRequest>);
            var _data_consumer_id = _o.DataConsumerId == null ? default(StringOffset) : builder.CreateString(_o.DataConsumerId);
            return CreateCloseDataConsumerRequest(
              builder,
              _data_consumer_id);
        }
    }

    public class CloseDataConsumerRequestT
    {
        [JsonPropertyName("data_consumer_id")]
        public string DataConsumerId { get; set; }

        public CloseDataConsumerRequestT()
        {
            this.DataConsumerId = null;
        }
    }
}
