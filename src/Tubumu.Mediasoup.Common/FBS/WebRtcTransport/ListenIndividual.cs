// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.WebRtcTransport
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct ListenIndividual : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static ListenIndividual GetRootAsListenIndividual(ByteBuffer _bb) { return GetRootAsListenIndividual(_bb, new ListenIndividual()); }
        public static ListenIndividual GetRootAsListenIndividual(ByteBuffer _bb, ListenIndividual obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public ListenIndividual __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.Transport.ListenInfo? ListenInfos(int j) { int o = __p.__offset(4); return o != 0 ? (FBS.Transport.ListenInfo?)(new FBS.Transport.ListenInfo()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int ListenInfosLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }

        public static Offset<FBS.WebRtcTransport.ListenIndividual> CreateListenIndividual(FlatBufferBuilder builder,
            VectorOffset listen_infosOffset = default(VectorOffset))
        {
            builder.StartTable(1);
            ListenIndividual.AddListenInfos(builder, listen_infosOffset);
            return ListenIndividual.EndListenIndividual(builder);
        }

        public static void StartListenIndividual(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddListenInfos(FlatBufferBuilder builder, VectorOffset listenInfosOffset) { builder.AddOffset(0, listenInfosOffset.Value, 0); }
        public static VectorOffset CreateListenInfosVector(FlatBufferBuilder builder, Offset<FBS.Transport.ListenInfo>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateListenInfosVectorBlock(FlatBufferBuilder builder, Offset<FBS.Transport.ListenInfo>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateListenInfosVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.Transport.ListenInfo>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateListenInfosVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.Transport.ListenInfo>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartListenInfosVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static Offset<FBS.WebRtcTransport.ListenIndividual> EndListenIndividual(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // listen_infos
            return new Offset<FBS.WebRtcTransport.ListenIndividual>(o);
        }
        public ListenIndividualT UnPack()
        {
            var _o = new ListenIndividualT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(ListenIndividualT _o)
        {
            _o.ListenInfos = new List<FBS.Transport.ListenInfoT>();
            for(var _j = 0; _j < this.ListenInfosLength; ++_j)
            { _o.ListenInfos.Add(this.ListenInfos(_j).HasValue ? this.ListenInfos(_j).Value.UnPack() : null); }
        }
        public static Offset<FBS.WebRtcTransport.ListenIndividual> Pack(FlatBufferBuilder builder, ListenIndividualT _o)
        {
            if(_o == null)
                return default(Offset<FBS.WebRtcTransport.ListenIndividual>);
            var _listen_infos = default(VectorOffset);
            if(_o.ListenInfos != null)
            {
                var __listen_infos = new Offset<FBS.Transport.ListenInfo>[_o.ListenInfos.Count];
                for(var _j = 0; _j < __listen_infos.Length; ++_j)
                { __listen_infos[_j] = FBS.Transport.ListenInfo.Pack(builder, _o.ListenInfos[_j]); }
                _listen_infos = CreateListenInfosVector(builder, __listen_infos);
            }
            return CreateListenIndividual(
              builder,
              _listen_infos);
        }
    }

    public class ListenIndividualT
    {
        public List<FBS.Transport.ListenInfoT> ListenInfos { get; set; }

        public ListenIndividualT()
        {
            this.ListenInfos = null;
        }
    }


    static public class ListenIndividualVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyVectorOfTables(tablePos, 4 /*ListenInfos*/, FBS.Transport.ListenInfoVerify.Verify, true)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
