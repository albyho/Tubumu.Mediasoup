// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.LibUring
{
    public struct Dump : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static Dump GetRootAsDump(ByteBuffer _bb) { return GetRootAsDump(_bb, new Dump()); }
        public static Dump GetRootAsDump(ByteBuffer _bb, Dump obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public Dump __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public ulong SqeProcessCount { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong SqeMissCount { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
        public ulong UserDataMissCount { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }

        public static Offset<FBS.LibUring.Dump> CreateDump(FlatBufferBuilder builder,
            ulong sqe_process_count = 0,
            ulong sqe_miss_count = 0,
            ulong user_data_miss_count = 0)
        {
            builder.StartTable(3);
            Dump.AddUserDataMissCount(builder, user_data_miss_count);
            Dump.AddSqeMissCount(builder, sqe_miss_count);
            Dump.AddSqeProcessCount(builder, sqe_process_count);
            return Dump.EndDump(builder);
        }

        public static void StartDump(FlatBufferBuilder builder) { builder.StartTable(3); }
        public static void AddSqeProcessCount(FlatBufferBuilder builder, ulong sqeProcessCount) { builder.AddUlong(0, sqeProcessCount, 0); }
        public static void AddSqeMissCount(FlatBufferBuilder builder, ulong sqeMissCount) { builder.AddUlong(1, sqeMissCount, 0); }
        public static void AddUserDataMissCount(FlatBufferBuilder builder, ulong userDataMissCount) { builder.AddUlong(2, userDataMissCount, 0); }
        public static Offset<FBS.LibUring.Dump> EndDump(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.LibUring.Dump>(o);
        }
        public DumpT UnPack()
        {
            var _o = new DumpT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(DumpT _o)
        {
            _o.SqeProcessCount = this.SqeProcessCount;
            _o.SqeMissCount = this.SqeMissCount;
            _o.UserDataMissCount = this.UserDataMissCount;
        }
        public static Offset<FBS.LibUring.Dump> Pack(FlatBufferBuilder builder, DumpT _o)
        {
            if(_o == null)
                return default(Offset<FBS.LibUring.Dump>);
            return CreateDump(
              builder,
              _o.SqeProcessCount,
              _o.SqeMissCount,
              _o.UserDataMissCount);
        }
    }
}
