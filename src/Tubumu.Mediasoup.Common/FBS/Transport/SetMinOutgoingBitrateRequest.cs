// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.Transport
{
    public struct SetMinOutgoingBitrateRequest : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static SetMinOutgoingBitrateRequest GetRootAsSetMinOutgoingBitrateRequest(ByteBuffer _bb) { return GetRootAsSetMinOutgoingBitrateRequest(_bb, new SetMinOutgoingBitrateRequest()); }
        public static SetMinOutgoingBitrateRequest GetRootAsSetMinOutgoingBitrateRequest(ByteBuffer _bb, SetMinOutgoingBitrateRequest obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public SetMinOutgoingBitrateRequest __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public uint MinOutgoingBitrate { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }

        public static Offset<FBS.Transport.SetMinOutgoingBitrateRequest> CreateSetMinOutgoingBitrateRequest(FlatBufferBuilder builder,
            uint min_outgoing_bitrate = 0)
        {
            builder.StartTable(1);
            SetMinOutgoingBitrateRequest.AddMinOutgoingBitrate(builder, min_outgoing_bitrate);
            return SetMinOutgoingBitrateRequest.EndSetMinOutgoingBitrateRequest(builder);
        }

        public static void StartSetMinOutgoingBitrateRequest(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddMinOutgoingBitrate(FlatBufferBuilder builder, uint minOutgoingBitrate) { builder.AddUint(0, minOutgoingBitrate, 0); }
        public static Offset<FBS.Transport.SetMinOutgoingBitrateRequest> EndSetMinOutgoingBitrateRequest(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.Transport.SetMinOutgoingBitrateRequest>(o);
        }
        public SetMinOutgoingBitrateRequestT UnPack()
        {
            var _o = new SetMinOutgoingBitrateRequestT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(SetMinOutgoingBitrateRequestT _o)
        {
            _o.MinOutgoingBitrate = this.MinOutgoingBitrate;
        }
        public static Offset<FBS.Transport.SetMinOutgoingBitrateRequest> Pack(FlatBufferBuilder builder, SetMinOutgoingBitrateRequestT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Transport.SetMinOutgoingBitrateRequest>);
            return CreateSetMinOutgoingBitrateRequest(
              builder,
              _o.MinOutgoingBitrate);
        }
    }

    public class SetMinOutgoingBitrateRequestT
    {
        [JsonPropertyName("min_outgoing_bitrate")]
        public uint MinOutgoingBitrate { get; set; }

        public SetMinOutgoingBitrateRequestT()
        {
            this.MinOutgoingBitrate = 0;
        }
    }
}