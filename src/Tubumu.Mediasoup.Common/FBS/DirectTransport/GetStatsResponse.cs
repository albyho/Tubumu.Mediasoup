// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.DirectTransport
{
    public struct GetStatsResponse : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static GetStatsResponse GetRootAsGetStatsResponse(ByteBuffer _bb) { return GetRootAsGetStatsResponse(_bb, new GetStatsResponse()); }
        public static GetStatsResponse GetRootAsGetStatsResponse(ByteBuffer _bb, GetStatsResponse obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public GetStatsResponse __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.Transport.Stats? Base { get { int o = __p.__offset(4); return o != 0 ? (FBS.Transport.Stats?)(new FBS.Transport.Stats()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }

        public static Offset<FBS.DirectTransport.GetStatsResponse> CreateGetStatsResponse(FlatBufferBuilder builder,
            Offset<FBS.Transport.Stats> @baseOffset = default(Offset<FBS.Transport.Stats>))
        {
            builder.StartTable(1);
            GetStatsResponse.AddBase(builder, @baseOffset);
            return GetStatsResponse.EndGetStatsResponse(builder);
        }

        public static void StartGetStatsResponse(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddBase(FlatBufferBuilder builder, Offset<FBS.Transport.Stats> baseOffset) { builder.AddOffset(0, baseOffset.Value, 0); }
        public static Offset<FBS.DirectTransport.GetStatsResponse> EndGetStatsResponse(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // base
            return new Offset<FBS.DirectTransport.GetStatsResponse>(o);
        }
        public GetStatsResponseT UnPack()
        {
            var _o = new GetStatsResponseT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(GetStatsResponseT _o)
        {
            _o.Base = this.Base.HasValue ? this.Base.Value.UnPack() : null;
        }
        public static Offset<FBS.DirectTransport.GetStatsResponse> Pack(FlatBufferBuilder builder, GetStatsResponseT _o)
        {
            if(_o == null)
                return default(Offset<FBS.DirectTransport.GetStatsResponse>);
            var _base = _o.Base == null ? default(Offset<FBS.Transport.Stats>) : FBS.Transport.Stats.Pack(builder, _o.Base);
            return CreateGetStatsResponse(
              builder,
              _base);
        }
    }
}
