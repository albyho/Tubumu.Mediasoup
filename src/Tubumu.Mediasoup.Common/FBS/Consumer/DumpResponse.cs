// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.Consumer
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct DumpResponse : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static DumpResponse GetRootAsDumpResponse(ByteBuffer _bb) { return GetRootAsDumpResponse(_bb, new DumpResponse()); }
        public static DumpResponse GetRootAsDumpResponse(ByteBuffer _bb, DumpResponse obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public DumpResponse __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.Consumer.ConsumerDump? Data { get { int o = __p.__offset(4); return o != 0 ? (FBS.Consumer.ConsumerDump?)(new FBS.Consumer.ConsumerDump()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }

        public static Offset<FBS.Consumer.DumpResponse> CreateDumpResponse(FlatBufferBuilder builder,
            Offset<FBS.Consumer.ConsumerDump> dataOffset = default(Offset<FBS.Consumer.ConsumerDump>))
        {
            builder.StartTable(1);
            DumpResponse.AddData(builder, dataOffset);
            return DumpResponse.EndDumpResponse(builder);
        }

        public static void StartDumpResponse(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddData(FlatBufferBuilder builder, Offset<FBS.Consumer.ConsumerDump> dataOffset) { builder.AddOffset(0, dataOffset.Value, 0); }
        public static Offset<FBS.Consumer.DumpResponse> EndDumpResponse(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // data
            return new Offset<FBS.Consumer.DumpResponse>(o);
        }
        public DumpResponseT UnPack()
        {
            var _o = new DumpResponseT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(DumpResponseT _o)
        {
            _o.Data = this.Data.HasValue ? this.Data.Value.UnPack() : null;
        }
        public static Offset<FBS.Consumer.DumpResponse> Pack(FlatBufferBuilder builder, DumpResponseT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Consumer.DumpResponse>);
            var _data = _o.Data == null ? default(Offset<FBS.Consumer.ConsumerDump>) : FBS.Consumer.ConsumerDump.Pack(builder, _o.Data);
            return CreateDumpResponse(
              builder,
              _data);
        }
    }

    public class DumpResponseT
    {
        public FBS.Consumer.ConsumerDumpT Data { get; set; }

        public DumpResponseT()
        {
            this.Data = null;
        }
    }


    static public class DumpResponseVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyTable(tablePos, 4 /*Data*/, FBS.Consumer.ConsumerDumpVerify.Verify, true)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
