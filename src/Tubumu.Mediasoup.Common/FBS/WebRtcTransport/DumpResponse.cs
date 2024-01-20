// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using System.Collections.Generic;
using Google.FlatBuffers;

namespace FBS.WebRtcTransport
{
    public struct DumpResponse : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static DumpResponse GetRootAsDumpResponse(ByteBuffer _bb) { return GetRootAsDumpResponse(_bb, new DumpResponse()); }
        public static DumpResponse GetRootAsDumpResponse(ByteBuffer _bb, DumpResponse obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public DumpResponse __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.Transport.Dump? Base { get { int o = __p.__offset(4); return o != 0 ? (FBS.Transport.Dump?)(new FBS.Transport.Dump()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public FBS.WebRtcTransport.IceRole IceRole { get { int o = __p.__offset(6); return o != 0 ? (FBS.WebRtcTransport.IceRole)__p.bb.Get(o + __p.bb_pos) : FBS.WebRtcTransport.IceRole.CONTROLLED; } }
        public FBS.WebRtcTransport.IceParameters? IceParameters { get { int o = __p.__offset(8); return o != 0 ? (FBS.WebRtcTransport.IceParameters?)(new FBS.WebRtcTransport.IceParameters()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public FBS.WebRtcTransport.IceCandidate? IceCandidates(int j) { int o = __p.__offset(10); return o != 0 ? (FBS.WebRtcTransport.IceCandidate?)(new FBS.WebRtcTransport.IceCandidate()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int IceCandidatesLength { get { int o = __p.__offset(10); return o != 0 ? __p.__vector_len(o) : 0; } }
        public FBS.WebRtcTransport.IceState IceState { get { int o = __p.__offset(12); return o != 0 ? (FBS.WebRtcTransport.IceState)__p.bb.Get(o + __p.bb_pos) : FBS.WebRtcTransport.IceState.NEW; } }
        public FBS.Transport.Tuple? IceSelectedTuple { get { int o = __p.__offset(14); return o != 0 ? (FBS.Transport.Tuple?)(new FBS.Transport.Tuple()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public FBS.WebRtcTransport.DtlsParameters? DtlsParameters { get { int o = __p.__offset(16); return o != 0 ? (FBS.WebRtcTransport.DtlsParameters?)(new FBS.WebRtcTransport.DtlsParameters()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public FBS.WebRtcTransport.DtlsState DtlsState { get { int o = __p.__offset(18); return o != 0 ? (FBS.WebRtcTransport.DtlsState)__p.bb.Get(o + __p.bb_pos) : FBS.WebRtcTransport.DtlsState.NEW; } }

        public static Offset<FBS.WebRtcTransport.DumpResponse> CreateDumpResponse(FlatBufferBuilder builder,
            Offset<FBS.Transport.Dump> @baseOffset = default(Offset<FBS.Transport.Dump>),
            FBS.WebRtcTransport.IceRole ice_role = FBS.WebRtcTransport.IceRole.CONTROLLED,
            Offset<FBS.WebRtcTransport.IceParameters> ice_parametersOffset = default(Offset<FBS.WebRtcTransport.IceParameters>),
            VectorOffset ice_candidatesOffset = default(VectorOffset),
            FBS.WebRtcTransport.IceState ice_state = FBS.WebRtcTransport.IceState.NEW,
            Offset<FBS.Transport.Tuple> ice_selected_tupleOffset = default(Offset<FBS.Transport.Tuple>),
            Offset<FBS.WebRtcTransport.DtlsParameters> dtls_parametersOffset = default(Offset<FBS.WebRtcTransport.DtlsParameters>),
            FBS.WebRtcTransport.DtlsState dtls_state = FBS.WebRtcTransport.DtlsState.NEW)
        {
            builder.StartTable(8);
            DumpResponse.AddDtlsParameters(builder, dtls_parametersOffset);
            DumpResponse.AddIceSelectedTuple(builder, ice_selected_tupleOffset);
            DumpResponse.AddIceCandidates(builder, ice_candidatesOffset);
            DumpResponse.AddIceParameters(builder, ice_parametersOffset);
            DumpResponse.AddBase(builder, @baseOffset);
            DumpResponse.AddDtlsState(builder, dtls_state);
            DumpResponse.AddIceState(builder, ice_state);
            DumpResponse.AddIceRole(builder, ice_role);
            return DumpResponse.EndDumpResponse(builder);
        }

        public static void StartDumpResponse(FlatBufferBuilder builder) { builder.StartTable(8); }
        public static void AddBase(FlatBufferBuilder builder, Offset<FBS.Transport.Dump> baseOffset) { builder.AddOffset(0, baseOffset.Value, 0); }
        public static void AddIceRole(FlatBufferBuilder builder, FBS.WebRtcTransport.IceRole iceRole) { builder.AddByte(1, (byte)iceRole, 0); }
        public static void AddIceParameters(FlatBufferBuilder builder, Offset<FBS.WebRtcTransport.IceParameters> iceParametersOffset) { builder.AddOffset(2, iceParametersOffset.Value, 0); }
        public static void AddIceCandidates(FlatBufferBuilder builder, VectorOffset iceCandidatesOffset) { builder.AddOffset(3, iceCandidatesOffset.Value, 0); }
        public static VectorOffset CreateIceCandidatesVector(FlatBufferBuilder builder, Offset<FBS.WebRtcTransport.IceCandidate>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateIceCandidatesVectorBlock(FlatBufferBuilder builder, Offset<FBS.WebRtcTransport.IceCandidate>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateIceCandidatesVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.WebRtcTransport.IceCandidate>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateIceCandidatesVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.WebRtcTransport.IceCandidate>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartIceCandidatesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddIceState(FlatBufferBuilder builder, FBS.WebRtcTransport.IceState iceState) { builder.AddByte(4, (byte)iceState, 0); }
        public static void AddIceSelectedTuple(FlatBufferBuilder builder, Offset<FBS.Transport.Tuple> iceSelectedTupleOffset) { builder.AddOffset(5, iceSelectedTupleOffset.Value, 0); }
        public static void AddDtlsParameters(FlatBufferBuilder builder, Offset<FBS.WebRtcTransport.DtlsParameters> dtlsParametersOffset) { builder.AddOffset(6, dtlsParametersOffset.Value, 0); }
        public static void AddDtlsState(FlatBufferBuilder builder, FBS.WebRtcTransport.DtlsState dtlsState) { builder.AddByte(7, (byte)dtlsState, 0); }
        public static Offset<FBS.WebRtcTransport.DumpResponse> EndDumpResponse(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // base
            builder.Required(o, 8);  // ice_parameters
            builder.Required(o, 10);  // ice_candidates
            builder.Required(o, 16);  // dtls_parameters
            return new Offset<FBS.WebRtcTransport.DumpResponse>(o);
        }
        public DumpResponseT UnPack()
        {
            var _o = new DumpResponseT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(DumpResponseT _o)
        {
            _o.Base = this.Base.HasValue ? this.Base.Value.UnPack() : null;
            _o.IceRole = this.IceRole;
            _o.IceParameters = this.IceParameters.HasValue ? this.IceParameters.Value.UnPack() : null;
            _o.IceCandidates = new List<FBS.WebRtcTransport.IceCandidateT>();
            for(var _j = 0; _j < this.IceCandidatesLength; ++_j)
            { _o.IceCandidates.Add(this.IceCandidates(_j).HasValue ? this.IceCandidates(_j).Value.UnPack() : null); }
            _o.IceState = this.IceState;
            _o.IceSelectedTuple = this.IceSelectedTuple.HasValue ? this.IceSelectedTuple.Value.UnPack() : null;
            _o.DtlsParameters = this.DtlsParameters.HasValue ? this.DtlsParameters.Value.UnPack() : null;
            _o.DtlsState = this.DtlsState;
        }
        public static Offset<FBS.WebRtcTransport.DumpResponse> Pack(FlatBufferBuilder builder, DumpResponseT _o)
        {
            if(_o == null)
                return default(Offset<FBS.WebRtcTransport.DumpResponse>);
            var _base = _o.Base == null ? default(Offset<FBS.Transport.Dump>) : FBS.Transport.Dump.Pack(builder, _o.Base);
            var _ice_parameters = _o.IceParameters == null ? default(Offset<FBS.WebRtcTransport.IceParameters>) : FBS.WebRtcTransport.IceParameters.Pack(builder, _o.IceParameters);
            var _ice_candidates = default(VectorOffset);
            if(_o.IceCandidates != null)
            {
                var __ice_candidates = new Offset<FBS.WebRtcTransport.IceCandidate>[_o.IceCandidates.Count];
                for(var _j = 0; _j < __ice_candidates.Length; ++_j)
                { __ice_candidates[_j] = FBS.WebRtcTransport.IceCandidate.Pack(builder, _o.IceCandidates[_j]); }
                _ice_candidates = CreateIceCandidatesVector(builder, __ice_candidates);
            }
            var _ice_selected_tuple = _o.IceSelectedTuple == null ? default(Offset<FBS.Transport.Tuple>) : FBS.Transport.Tuple.Pack(builder, _o.IceSelectedTuple);
            var _dtls_parameters = _o.DtlsParameters == null ? default(Offset<FBS.WebRtcTransport.DtlsParameters>) : FBS.WebRtcTransport.DtlsParameters.Pack(builder, _o.DtlsParameters);
            return CreateDumpResponse(
              builder,
              _base,
              _o.IceRole,
              _ice_parameters,
              _ice_candidates,
              _o.IceState,
              _ice_selected_tuple,
              _dtls_parameters,
              _o.DtlsState);
        }
    }
}
