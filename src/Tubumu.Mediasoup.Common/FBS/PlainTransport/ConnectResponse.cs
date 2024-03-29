// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FBS.PlainTransport
{
    using Google.FlatBuffers;
    using System.Text.Json.Serialization;

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
        public FBS.Transport.Tuple? RtcpTuple { get { int o = __p.__offset(6); return o != 0 ? (FBS.Transport.Tuple?)(new FBS.Transport.Tuple()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public FBS.SrtpParameters.SrtpParameters? SrtpParameters { get { int o = __p.__offset(8); return o != 0 ? (FBS.SrtpParameters.SrtpParameters?)(new FBS.SrtpParameters.SrtpParameters()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }

        public static Offset<FBS.PlainTransport.ConnectResponse> CreateConnectResponse(FlatBufferBuilder builder,
            Offset<FBS.Transport.Tuple> tupleOffset = default(Offset<FBS.Transport.Tuple>),
            Offset<FBS.Transport.Tuple> rtcp_tupleOffset = default(Offset<FBS.Transport.Tuple>),
            Offset<FBS.SrtpParameters.SrtpParameters> srtp_parametersOffset = default(Offset<FBS.SrtpParameters.SrtpParameters>))
        {
            builder.StartTable(3);
            ConnectResponse.AddSrtpParameters(builder, srtp_parametersOffset);
            ConnectResponse.AddRtcpTuple(builder, rtcp_tupleOffset);
            ConnectResponse.AddTuple(builder, tupleOffset);
            return ConnectResponse.EndConnectResponse(builder);
        }

        public static void StartConnectResponse(FlatBufferBuilder builder) { builder.StartTable(3); }
        public static void AddTuple(FlatBufferBuilder builder, Offset<FBS.Transport.Tuple> tupleOffset) { builder.AddOffset(0, tupleOffset.Value, 0); }
        public static void AddRtcpTuple(FlatBufferBuilder builder, Offset<FBS.Transport.Tuple> rtcpTupleOffset) { builder.AddOffset(1, rtcpTupleOffset.Value, 0); }
        public static void AddSrtpParameters(FlatBufferBuilder builder, Offset<FBS.SrtpParameters.SrtpParameters> srtpParametersOffset) { builder.AddOffset(2, srtpParametersOffset.Value, 0); }
        public static Offset<FBS.PlainTransport.ConnectResponse> EndConnectResponse(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // tuple
            return new Offset<FBS.PlainTransport.ConnectResponse>(o);
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
            _o.RtcpTuple = this.RtcpTuple.HasValue ? this.RtcpTuple.Value.UnPack() : null;
            _o.SrtpParameters = this.SrtpParameters.HasValue ? this.SrtpParameters.Value.UnPack() : null;
        }
        public static Offset<FBS.PlainTransport.ConnectResponse> Pack(FlatBufferBuilder builder, ConnectResponseT _o)
        {
            if(_o == null)
                return default(Offset<FBS.PlainTransport.ConnectResponse>);
            var _tuple = _o.Tuple == null ? default(Offset<FBS.Transport.Tuple>) : FBS.Transport.Tuple.Pack(builder, _o.Tuple);
            var _rtcp_tuple = _o.RtcpTuple == null ? default(Offset<FBS.Transport.Tuple>) : FBS.Transport.Tuple.Pack(builder, _o.RtcpTuple);
            var _srtp_parameters = _o.SrtpParameters == null ? default(Offset<FBS.SrtpParameters.SrtpParameters>) : FBS.SrtpParameters.SrtpParameters.Pack(builder, _o.SrtpParameters);
            return CreateConnectResponse(
              builder,
              _tuple,
              _rtcp_tuple,
              _srtp_parameters);
        }
    }
}
