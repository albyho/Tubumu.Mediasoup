// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.Worker
{
    public struct ResourceUsageResponse : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static ResourceUsageResponse GetRootAsResourceUsageResponse(ByteBuffer _bb) { return GetRootAsResourceUsageResponse(_bb, new ResourceUsageResponse()); }
        public static ResourceUsageResponse GetRootAsResourceUsageResponse(ByteBuffer _bb, ResourceUsageResponse obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public ResourceUsageResponse __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public ulong RuUtime { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong RuStime { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong RuMaxrss { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong RuIxrss { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong RuIdrss { get { int o = __p.__offset(12); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong RuIsrss { get { int o = __p.__offset(14); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong RuMinflt { get { int o = __p.__offset(16); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong RuMajflt { get { int o = __p.__offset(18); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong RuNswap { get { int o = __p.__offset(20); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong RuInblock { get { int o = __p.__offset(22); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong RuOublock { get { int o = __p.__offset(24); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong RuMsgsnd { get { int o = __p.__offset(26); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong RuMsgrcv { get { int o = __p.__offset(28); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong RuNsignals { get { int o = __p.__offset(30); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong RuNvcsw { get { int o = __p.__offset(32); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong RuNivcsw { get { int o = __p.__offset(34); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }

        public static Offset<FBS.Worker.ResourceUsageResponse> CreateResourceUsageResponse(FlatBufferBuilder builder,
            ulong ru_utime = 0,
            ulong ru_stime = 0,
            ulong ru_maxrss = 0,
            ulong ru_ixrss = 0,
            ulong ru_idrss = 0,
            ulong ru_isrss = 0,
            ulong ru_minflt = 0,
            ulong ru_majflt = 0,
            ulong ru_nswap = 0,
            ulong ru_inblock = 0,
            ulong ru_oublock = 0,
            ulong ru_msgsnd = 0,
            ulong ru_msgrcv = 0,
            ulong ru_nsignals = 0,
            ulong ru_nvcsw = 0,
            ulong ru_nivcsw = 0)
        {
            builder.StartTable(16);
            ResourceUsageResponse.AddRuNivcsw(builder, ru_nivcsw);
            ResourceUsageResponse.AddRuNvcsw(builder, ru_nvcsw);
            ResourceUsageResponse.AddRuNsignals(builder, ru_nsignals);
            ResourceUsageResponse.AddRuMsgrcv(builder, ru_msgrcv);
            ResourceUsageResponse.AddRuMsgsnd(builder, ru_msgsnd);
            ResourceUsageResponse.AddRuOublock(builder, ru_oublock);
            ResourceUsageResponse.AddRuInblock(builder, ru_inblock);
            ResourceUsageResponse.AddRuNswap(builder, ru_nswap);
            ResourceUsageResponse.AddRuMajflt(builder, ru_majflt);
            ResourceUsageResponse.AddRuMinflt(builder, ru_minflt);
            ResourceUsageResponse.AddRuIsrss(builder, ru_isrss);
            ResourceUsageResponse.AddRuIdrss(builder, ru_idrss);
            ResourceUsageResponse.AddRuIxrss(builder, ru_ixrss);
            ResourceUsageResponse.AddRuMaxrss(builder, ru_maxrss);
            ResourceUsageResponse.AddRuStime(builder, ru_stime);
            ResourceUsageResponse.AddRuUtime(builder, ru_utime);
            return ResourceUsageResponse.EndResourceUsageResponse(builder);
        }

        public static void StartResourceUsageResponse(FlatBufferBuilder builder) { builder.StartTable(16); }
        public static void AddRuUtime(FlatBufferBuilder builder, ulong ruUtime) { builder.AddUlong(0, ruUtime, 0); }
        public static void AddRuStime(FlatBufferBuilder builder, ulong ruStime) { builder.AddUlong(1, ruStime, 0); }
        public static void AddRuMaxrss(FlatBufferBuilder builder, ulong ruMaxrss) { builder.AddUlong(2, ruMaxrss, 0); }
        public static void AddRuIxrss(FlatBufferBuilder builder, ulong ruIxrss) { builder.AddUlong(3, ruIxrss, 0); }
        public static void AddRuIdrss(FlatBufferBuilder builder, ulong ruIdrss) { builder.AddUlong(4, ruIdrss, 0); }
        public static void AddRuIsrss(FlatBufferBuilder builder, ulong ruIsrss) { builder.AddUlong(5, ruIsrss, 0); }
        public static void AddRuMinflt(FlatBufferBuilder builder, ulong ruMinflt) { builder.AddUlong(6, ruMinflt, 0); }
        public static void AddRuMajflt(FlatBufferBuilder builder, ulong ruMajflt) { builder.AddUlong(7, ruMajflt, 0); }
        public static void AddRuNswap(FlatBufferBuilder builder, ulong ruNswap) { builder.AddUlong(8, ruNswap, 0); }
        public static void AddRuInblock(FlatBufferBuilder builder, ulong ruInblock) { builder.AddUlong(9, ruInblock, 0); }
        public static void AddRuOublock(FlatBufferBuilder builder, ulong ruOublock) { builder.AddUlong(10, ruOublock, 0); }
        public static void AddRuMsgsnd(FlatBufferBuilder builder, ulong ruMsgsnd) { builder.AddUlong(11, ruMsgsnd, 0); }
        public static void AddRuMsgrcv(FlatBufferBuilder builder, ulong ruMsgrcv) { builder.AddUlong(12, ruMsgrcv, 0); }
        public static void AddRuNsignals(FlatBufferBuilder builder, ulong ruNsignals) { builder.AddUlong(13, ruNsignals, 0); }
        public static void AddRuNvcsw(FlatBufferBuilder builder, ulong ruNvcsw) { builder.AddUlong(14, ruNvcsw, 0); }
        public static void AddRuNivcsw(FlatBufferBuilder builder, ulong ruNivcsw) { builder.AddUlong(15, ruNivcsw, 0); }
        public static Offset<FBS.Worker.ResourceUsageResponse> EndResourceUsageResponse(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.Worker.ResourceUsageResponse>(o);
        }
        public ResourceUsageResponseT UnPack()
        {
            var _o = new ResourceUsageResponseT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(ResourceUsageResponseT _o)
        {
            _o.RuUtime = this.RuUtime;
            _o.RuStime = this.RuStime;
            _o.RuMaxrss = this.RuMaxrss;
            _o.RuIxrss = this.RuIxrss;
            _o.RuIdrss = this.RuIdrss;
            _o.RuIsrss = this.RuIsrss;
            _o.RuMinflt = this.RuMinflt;
            _o.RuMajflt = this.RuMajflt;
            _o.RuNswap = this.RuNswap;
            _o.RuInblock = this.RuInblock;
            _o.RuOublock = this.RuOublock;
            _o.RuMsgsnd = this.RuMsgsnd;
            _o.RuMsgrcv = this.RuMsgrcv;
            _o.RuNsignals = this.RuNsignals;
            _o.RuNvcsw = this.RuNvcsw;
            _o.RuNivcsw = this.RuNivcsw;
        }
        public static Offset<FBS.Worker.ResourceUsageResponse> Pack(FlatBufferBuilder builder, ResourceUsageResponseT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Worker.ResourceUsageResponse>);
            return CreateResourceUsageResponse(
              builder,
              _o.RuUtime,
              _o.RuStime,
              _o.RuMaxrss,
              _o.RuIxrss,
              _o.RuIdrss,
              _o.RuIsrss,
              _o.RuMinflt,
              _o.RuMajflt,
              _o.RuNswap,
              _o.RuInblock,
              _o.RuOublock,
              _o.RuMsgsnd,
              _o.RuMsgrcv,
              _o.RuNsignals,
              _o.RuNvcsw,
              _o.RuNivcsw);
        }
    }
}
