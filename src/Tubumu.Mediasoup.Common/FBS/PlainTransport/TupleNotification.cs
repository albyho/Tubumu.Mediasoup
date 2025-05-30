// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.PlainTransport
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct TupleNotification : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static TupleNotification GetRootAsTupleNotification(ByteBuffer _bb) { return GetRootAsTupleNotification(_bb, new TupleNotification()); }
        public static TupleNotification GetRootAsTupleNotification(ByteBuffer _bb, TupleNotification obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public TupleNotification __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.Transport.Tuple? Tuple { get { int o = __p.__offset(4); return o != 0 ? (FBS.Transport.Tuple?)(new FBS.Transport.Tuple()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }

        public static Offset<FBS.PlainTransport.TupleNotification> CreateTupleNotification(FlatBufferBuilder builder,
            Offset<FBS.Transport.Tuple> tupleOffset = default(Offset<FBS.Transport.Tuple>))
        {
            builder.StartTable(1);
            TupleNotification.AddTuple(builder, tupleOffset);
            return TupleNotification.EndTupleNotification(builder);
        }

        public static void StartTupleNotification(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddTuple(FlatBufferBuilder builder, Offset<FBS.Transport.Tuple> tupleOffset) { builder.AddOffset(0, tupleOffset.Value, 0); }
        public static Offset<FBS.PlainTransport.TupleNotification> EndTupleNotification(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // tuple
            return new Offset<FBS.PlainTransport.TupleNotification>(o);
        }
        public TupleNotificationT UnPack()
        {
            var _o = new TupleNotificationT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(TupleNotificationT _o)
        {
            _o.Tuple = this.Tuple.HasValue ? this.Tuple.Value.UnPack() : null;
        }
        public static Offset<FBS.PlainTransport.TupleNotification> Pack(FlatBufferBuilder builder, TupleNotificationT _o)
        {
            if(_o == null)
                return default(Offset<FBS.PlainTransport.TupleNotification>);
            var _tuple = _o.Tuple == null ? default(Offset<FBS.Transport.Tuple>) : FBS.Transport.Tuple.Pack(builder, _o.Tuple);
            return CreateTupleNotification(
              builder,
              _tuple);
        }
    }

    public class TupleNotificationT
    {
        public FBS.Transport.TupleT Tuple { get; set; }

        public TupleNotificationT()
        {
            this.Tuple = null;
        }
    }


    static public class TupleNotificationVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyTable(tablePos, 4 /*Tuple*/, FBS.Transport.TupleVerify.Verify, true)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
