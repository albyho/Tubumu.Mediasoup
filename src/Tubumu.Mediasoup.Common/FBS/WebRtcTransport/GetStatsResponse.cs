// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.WebRtcTransport
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct GetStatsResponse : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static GetStatsResponse GetRootAsGetStatsResponse(ByteBuffer _bb) { return GetRootAsGetStatsResponse(_bb, new GetStatsResponse()); }
        public static GetStatsResponse GetRootAsGetStatsResponse(ByteBuffer _bb, GetStatsResponse obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public GetStatsResponse __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.Transport.Stats? Base { get { int o = __p.__offset(4); return o != 0 ? (FBS.Transport.Stats?)(new FBS.Transport.Stats()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public FBS.WebRtcTransport.IceRole IceRole { get { int o = __p.__offset(6); return o != 0 ? (FBS.WebRtcTransport.IceRole)__p.bb.Get(o + __p.bb_pos) : FBS.WebRtcTransport.IceRole.CONTROLLED; } }
        public FBS.WebRtcTransport.IceState IceState { get { int o = __p.__offset(8); return o != 0 ? (FBS.WebRtcTransport.IceState)__p.bb.Get(o + __p.bb_pos) : FBS.WebRtcTransport.IceState.NEW; } }
        public FBS.Transport.Tuple? IceSelectedTuple { get { int o = __p.__offset(10); return o != 0 ? (FBS.Transport.Tuple?)(new FBS.Transport.Tuple()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public FBS.WebRtcTransport.DtlsState DtlsState { get { int o = __p.__offset(12); return o != 0 ? (FBS.WebRtcTransport.DtlsState)__p.bb.Get(o + __p.bb_pos) : FBS.WebRtcTransport.DtlsState.NEW; } }

        public static Offset<FBS.WebRtcTransport.GetStatsResponse> CreateGetStatsResponse(FlatBufferBuilder builder,
            Offset<FBS.Transport.Stats> @baseOffset = default(Offset<FBS.Transport.Stats>),
            FBS.WebRtcTransport.IceRole ice_role = FBS.WebRtcTransport.IceRole.CONTROLLED,
            FBS.WebRtcTransport.IceState ice_state = FBS.WebRtcTransport.IceState.NEW,
            Offset<FBS.Transport.Tuple> ice_selected_tupleOffset = default(Offset<FBS.Transport.Tuple>),
            FBS.WebRtcTransport.DtlsState dtls_state = FBS.WebRtcTransport.DtlsState.NEW)
        {
            builder.StartTable(5);
            GetStatsResponse.AddIceSelectedTuple(builder, ice_selected_tupleOffset);
            GetStatsResponse.AddBase(builder, @baseOffset);
            GetStatsResponse.AddDtlsState(builder, dtls_state);
            GetStatsResponse.AddIceState(builder, ice_state);
            GetStatsResponse.AddIceRole(builder, ice_role);
            return GetStatsResponse.EndGetStatsResponse(builder);
        }

        public static void StartGetStatsResponse(FlatBufferBuilder builder) { builder.StartTable(5); }
        public static void AddBase(FlatBufferBuilder builder, Offset<FBS.Transport.Stats> baseOffset) { builder.AddOffset(0, baseOffset.Value, 0); }
        public static void AddIceRole(FlatBufferBuilder builder, FBS.WebRtcTransport.IceRole iceRole) { builder.AddByte(1, (byte)iceRole, 0); }
        public static void AddIceState(FlatBufferBuilder builder, FBS.WebRtcTransport.IceState iceState) { builder.AddByte(2, (byte)iceState, 0); }
        public static void AddIceSelectedTuple(FlatBufferBuilder builder, Offset<FBS.Transport.Tuple> iceSelectedTupleOffset) { builder.AddOffset(3, iceSelectedTupleOffset.Value, 0); }
        public static void AddDtlsState(FlatBufferBuilder builder, FBS.WebRtcTransport.DtlsState dtlsState) { builder.AddByte(4, (byte)dtlsState, 0); }
        public static Offset<FBS.WebRtcTransport.GetStatsResponse> EndGetStatsResponse(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // base
            return new Offset<FBS.WebRtcTransport.GetStatsResponse>(o);
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
            _o.IceRole = this.IceRole;
            _o.IceState = this.IceState;
            _o.IceSelectedTuple = this.IceSelectedTuple.HasValue ? this.IceSelectedTuple.Value.UnPack() : null;
            _o.DtlsState = this.DtlsState;
        }
        public static Offset<FBS.WebRtcTransport.GetStatsResponse> Pack(FlatBufferBuilder builder, GetStatsResponseT _o)
        {
            if(_o == null)
                return default(Offset<FBS.WebRtcTransport.GetStatsResponse>);
            var _base = _o.Base == null ? default(Offset<FBS.Transport.Stats>) : FBS.Transport.Stats.Pack(builder, _o.Base);
            var _ice_selected_tuple = _o.IceSelectedTuple == null ? default(Offset<FBS.Transport.Tuple>) : FBS.Transport.Tuple.Pack(builder, _o.IceSelectedTuple);
            return CreateGetStatsResponse(
              builder,
              _base,
              _o.IceRole,
              _o.IceState,
              _ice_selected_tuple,
              _o.DtlsState);
        }
    }

    public class GetStatsResponseT
    {
        public FBS.Transport.StatsT Base { get; set; }

        public FBS.WebRtcTransport.IceRole IceRole { get; set; }

        public FBS.WebRtcTransport.IceState IceState { get; set; }

        public FBS.Transport.TupleT IceSelectedTuple { get; set; }

        public FBS.WebRtcTransport.DtlsState DtlsState { get; set; }

        public GetStatsResponseT()
        {
            this.Base = null;
            this.IceRole = FBS.WebRtcTransport.IceRole.CONTROLLED;
            this.IceState = FBS.WebRtcTransport.IceState.NEW;
            this.IceSelectedTuple = null;
            this.DtlsState = FBS.WebRtcTransport.DtlsState.NEW;
        }
    }


    static public class GetStatsResponseVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyTable(tablePos, 4 /*Base*/, FBS.Transport.StatsVerify.Verify, true)
              && verifier.VerifyField(tablePos, 6 /*IceRole*/, 1 /*FBS.WebRtcTransport.IceRole*/, 1, false)
              && verifier.VerifyField(tablePos, 8 /*IceState*/, 1 /*FBS.WebRtcTransport.IceState*/, 1, false)
              && verifier.VerifyTable(tablePos, 10 /*IceSelectedTuple*/, FBS.Transport.TupleVerify.Verify, false)
              && verifier.VerifyField(tablePos, 12 /*DtlsState*/, 1 /*FBS.WebRtcTransport.DtlsState*/, 1, false)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
