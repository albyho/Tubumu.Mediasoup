// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using Google.FlatBuffers;

namespace FBS.RtpParameters
{
    public struct Rtx : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static Rtx GetRootAsRtx(ByteBuffer _bb) { return GetRootAsRtx(_bb, new Rtx()); }
        public static Rtx GetRootAsRtx(ByteBuffer _bb, Rtx obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public Rtx __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public uint Ssrc { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }

        public static Offset<FBS.RtpParameters.Rtx> CreateRtx(FlatBufferBuilder builder,
            uint ssrc = 0)
        {
            builder.StartTable(1);
            Rtx.AddSsrc(builder, ssrc);
            return Rtx.EndRtx(builder);
        }

        public static void StartRtx(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddSsrc(FlatBufferBuilder builder, uint ssrc) { builder.AddUint(0, ssrc, 0); }
        public static Offset<FBS.RtpParameters.Rtx> EndRtx(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.RtpParameters.Rtx>(o);
        }
        public RtxT UnPack()
        {
            var _o = new RtxT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(RtxT _o)
        {
            _o.Ssrc = this.Ssrc;
        }
        public static Offset<FBS.RtpParameters.Rtx> Pack(FlatBufferBuilder builder, RtxT _o)
        {
            if(_o == null)
                return default(Offset<FBS.RtpParameters.Rtx>);
            return CreateRtx(
              builder,
              _o.Ssrc);
        }
    }
}
