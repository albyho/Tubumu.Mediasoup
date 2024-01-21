// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.PipeTransport
{
    public struct ConnectResponse : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static ConnectResponse GetRootAsConnectResponse(ByteBuffer _bb) { return GetRootAsConnectResponse(_bb, new ConnectResponse()); }
        public static ConnectResponse GetRootAsConnectResponse(ByteBuffer _bb, ConnectResponse obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public ConnectResponse __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.Transport.Tuple? Tuple { get { int o = __p.__offset(4); return o != 0 ? (FBS.Transport.Tuple?)(new FBS.Transport.Tuple()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }

        public static Offset<FBS.PipeTransport.ConnectResponse> CreateConnectResponse(FlatBufferBuilder builder,
            Offset<FBS.Transport.Tuple> tupleOffset = default(Offset<FBS.Transport.Tuple>))
        {
            builder.StartTable(1);
            ConnectResponse.AddTuple(builder, tupleOffset);
            return ConnectResponse.EndConnectResponse(builder);
        }

        public static void StartConnectResponse(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddTuple(FlatBufferBuilder builder, Offset<FBS.Transport.Tuple> tupleOffset) { builder.AddOffset(0, tupleOffset.Value, 0); }
        public static Offset<FBS.PipeTransport.ConnectResponse> EndConnectResponse(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // tuple
            return new Offset<FBS.PipeTransport.ConnectResponse>(o);
        }
        public ConnectResponseT UnPack()
        {
            var _o = new ConnectResponseT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(ConnectResponseT _o)
        {
            _o.Tuple = this.Tuple.HasValue ? this.Tuple.Value.UnPack() : null;
        }
        public static Offset<FBS.PipeTransport.ConnectResponse> Pack(FlatBufferBuilder builder, ConnectResponseT _o)
        {
            if(_o == null)
                return default(Offset<FBS.PipeTransport.ConnectResponse>);
            var _tuple = _o.Tuple == null ? default(Offset<FBS.Transport.Tuple>) : FBS.Transport.Tuple.Pack(builder, _o.Tuple);
            return CreateConnectResponse(
              builder,
              _tuple);
        }
    }
}
