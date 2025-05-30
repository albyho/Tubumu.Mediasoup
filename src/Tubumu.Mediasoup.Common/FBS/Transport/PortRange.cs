// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.Transport
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct PortRange : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static PortRange GetRootAsPortRange(ByteBuffer _bb) { return GetRootAsPortRange(_bb, new PortRange()); }
        public static PortRange GetRootAsPortRange(ByteBuffer _bb, PortRange obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public PortRange __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public ushort Min { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUshort(o + __p.bb_pos) : (ushort)0; } }
        public ushort Max { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUshort(o + __p.bb_pos) : (ushort)0; } }

        public static Offset<FBS.Transport.PortRange> CreatePortRange(FlatBufferBuilder builder,
            ushort min = 0,
            ushort max = 0)
        {
            builder.StartTable(2);
            PortRange.AddMax(builder, max);
            PortRange.AddMin(builder, min);
            return PortRange.EndPortRange(builder);
        }

        public static void StartPortRange(FlatBufferBuilder builder) { builder.StartTable(2); }
        public static void AddMin(FlatBufferBuilder builder, ushort min) { builder.AddUshort(0, min, 0); }
        public static void AddMax(FlatBufferBuilder builder, ushort max) { builder.AddUshort(1, max, 0); }
        public static Offset<FBS.Transport.PortRange> EndPortRange(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.Transport.PortRange>(o);
        }
        public PortRangeT UnPack()
        {
            var _o = new PortRangeT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(PortRangeT _o)
        {
            _o.Min = this.Min;
            _o.Max = this.Max;
        }
        public static Offset<FBS.Transport.PortRange> Pack(FlatBufferBuilder builder, PortRangeT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Transport.PortRange>);
            return CreatePortRange(
              builder,
              _o.Min,
              _o.Max);
        }
    }

    public class PortRangeT
    {
        public ushort Min { get; set; }

        public ushort Max { get; set; }

        public PortRangeT()
        {
            this.Min = 0;
            this.Max = 0;
        }
    }


    static public class PortRangeVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyField(tablePos, 4 /*Min*/, 2 /*ushort*/, 2, false)
              && verifier.VerifyField(tablePos, 6 /*Max*/, 2 /*ushort*/, 2, false)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
