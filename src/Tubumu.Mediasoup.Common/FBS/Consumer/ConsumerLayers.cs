// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.Consumer
{
    public struct ConsumerLayers : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static ConsumerLayers GetRootAsConsumerLayers(ByteBuffer _bb) { return GetRootAsConsumerLayers(_bb, new ConsumerLayers()); }
        public static ConsumerLayers GetRootAsConsumerLayers(ByteBuffer _bb, ConsumerLayers obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public ConsumerLayers __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public byte SpatialLayer { get { int o = __p.__offset(4); return o != 0 ? __p.bb.Get(o + __p.bb_pos) : (byte)0; } }
        public byte? TemporalLayer { get { int o = __p.__offset(6); return o != 0 ? __p.bb.Get(o + __p.bb_pos) : (byte?)null; } }

        public static Offset<FBS.Consumer.ConsumerLayers> CreateConsumerLayers(FlatBufferBuilder builder,
            byte spatial_layer = 0,
            byte? temporal_layer = null)
        {
            builder.StartTable(2);
            ConsumerLayers.AddTemporalLayer(builder, temporal_layer);
            ConsumerLayers.AddSpatialLayer(builder, spatial_layer);
            return ConsumerLayers.EndConsumerLayers(builder);
        }

        public static void StartConsumerLayers(FlatBufferBuilder builder) { builder.StartTable(2); }
        public static void AddSpatialLayer(FlatBufferBuilder builder, byte spatialLayer) { builder.AddByte(0, spatialLayer, 0); }
        public static void AddTemporalLayer(FlatBufferBuilder builder, byte? temporalLayer) { builder.AddByte(1, temporalLayer); }
        public static Offset<FBS.Consumer.ConsumerLayers> EndConsumerLayers(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.Consumer.ConsumerLayers>(o);
        }
        public ConsumerLayersT UnPack()
        {
            var _o = new ConsumerLayersT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(ConsumerLayersT _o)
        {
            _o.SpatialLayer = this.SpatialLayer;
            _o.TemporalLayer = this.TemporalLayer;
        }
        public static Offset<FBS.Consumer.ConsumerLayers> Pack(FlatBufferBuilder builder, ConsumerLayersT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Consumer.ConsumerLayers>);
            return CreateConsumerLayers(
              builder,
              _o.SpatialLayer,
              _o.TemporalLayer);
        }
    }

    public class ConsumerLayersT
    {
        /// <summary>
        /// spatialLayer. Renamed.
        /// </summary>
        [JsonPropertyName("spatialLayer")]
        public byte SpatialLayer { get; set; }

        /// <summary>
        /// temporalLayer. Renamed.
        /// </summary>
        [JsonPropertyName("temporalLayer")]
        public byte? TemporalLayer { get; set; }

        public ConsumerLayersT()
        {
            this.SpatialLayer = 0;
            this.TemporalLayer = null;
        }
    }
}
