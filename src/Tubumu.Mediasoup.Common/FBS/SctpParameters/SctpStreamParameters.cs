// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.SctpParameters
{
    public struct SctpStreamParameters : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static SctpStreamParameters GetRootAsSctpStreamParameters(ByteBuffer _bb) { return GetRootAsSctpStreamParameters(_bb, new SctpStreamParameters()); }
        public static SctpStreamParameters GetRootAsSctpStreamParameters(ByteBuffer _bb, SctpStreamParameters obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public SctpStreamParameters __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public ushort StreamId { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUshort(o + __p.bb_pos) : (ushort)0; } }
        public bool? Ordered { get { int o = __p.__offset(6); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool?)null; } }
        public ushort? MaxPacketLifeTime { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetUshort(o + __p.bb_pos) : (ushort?)null; } }
        public ushort? MaxRetransmits { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetUshort(o + __p.bb_pos) : (ushort?)null; } }

        public static Offset<FBS.SctpParameters.SctpStreamParameters> CreateSctpStreamParameters(FlatBufferBuilder builder,
            ushort stream_id = 0,
            bool? ordered = null,
            ushort? max_packet_life_time = null,
            ushort? max_retransmits = null)
        {
            builder.StartTable(4);
            SctpStreamParameters.AddMaxRetransmits(builder, max_retransmits);
            SctpStreamParameters.AddMaxPacketLifeTime(builder, max_packet_life_time);
            SctpStreamParameters.AddStreamId(builder, stream_id);
            SctpStreamParameters.AddOrdered(builder, ordered);
            return SctpStreamParameters.EndSctpStreamParameters(builder);
        }

        public static void StartSctpStreamParameters(FlatBufferBuilder builder) { builder.StartTable(4); }
        public static void AddStreamId(FlatBufferBuilder builder, ushort streamId) { builder.AddUshort(0, streamId, 0); }
        public static void AddOrdered(FlatBufferBuilder builder, bool? ordered) { builder.AddBool(1, ordered); }
        public static void AddMaxPacketLifeTime(FlatBufferBuilder builder, ushort? maxPacketLifeTime) { builder.AddUshort(2, maxPacketLifeTime); }
        public static void AddMaxRetransmits(FlatBufferBuilder builder, ushort? maxRetransmits) { builder.AddUshort(3, maxRetransmits); }
        public static Offset<FBS.SctpParameters.SctpStreamParameters> EndSctpStreamParameters(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.SctpParameters.SctpStreamParameters>(o);
        }
        public SctpStreamParametersT UnPack()
        {
            var _o = new SctpStreamParametersT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(SctpStreamParametersT _o)
        {
            _o.StreamId = this.StreamId;
            _o.Ordered = this.Ordered;
            _o.MaxPacketLifeTime = this.MaxPacketLifeTime;
            _o.MaxRetransmits = this.MaxRetransmits;
        }
        public static Offset<FBS.SctpParameters.SctpStreamParameters> Pack(FlatBufferBuilder builder, SctpStreamParametersT _o)
        {
            if(_o == null)
                return default(Offset<FBS.SctpParameters.SctpStreamParameters>);
            return CreateSctpStreamParameters(
              builder,
              _o.StreamId,
              _o.Ordered,
              _o.MaxPacketLifeTime,
              _o.MaxRetransmits);
        }
    }

    public class SctpStreamParametersT
    {
        /// <summary>
        /// streamId. Renamed.
        /// </summary>
        [JsonPropertyName("streamId")]
        public ushort StreamId { get; set; }

        /// <summary>
        /// ordered
        /// </summary>
        [JsonPropertyName("ordered")]
        public bool? Ordered { get; set; }

        /// <summary>
        /// maxPacketLifeTime. Renamed.
        /// </summary>
        [JsonPropertyName("maxPacketLifeTime")]
        public ushort? MaxPacketLifeTime { get; set; }

        /// <summary>
        /// maxRetransmits. Renamed.
        /// </summary>
        [JsonPropertyName("maxRetransmits")]
        public ushort? MaxRetransmits { get; set; }

        public SctpStreamParametersT()
        {
            this.StreamId = 0;
            this.Ordered = null;
            this.MaxPacketLifeTime = null;
            this.MaxRetransmits = null;
        }
    }
}
