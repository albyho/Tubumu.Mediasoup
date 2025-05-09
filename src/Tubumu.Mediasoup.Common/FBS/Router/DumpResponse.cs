// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.Router
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

        public string Id { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetIdArray() { return __p.__vector_as_array<byte>(4); }
        public string TransportIds(int j) { int o = __p.__offset(6); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : null; }
        public int TransportIdsLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }
        public string RtpObserverIds(int j) { int o = __p.__offset(8); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : null; }
        public int RtpObserverIdsLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
        public FBS.Common.StringStringArray? MapProducerIdConsumerIds(int j) { int o = __p.__offset(10); return o != 0 ? (FBS.Common.StringStringArray?)(new FBS.Common.StringStringArray()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int MapProducerIdConsumerIdsLength { get { int o = __p.__offset(10); return o != 0 ? __p.__vector_len(o) : 0; } }
        public FBS.Common.StringString? MapConsumerIdProducerId(int j) { int o = __p.__offset(12); return o != 0 ? (FBS.Common.StringString?)(new FBS.Common.StringString()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int MapConsumerIdProducerIdLength { get { int o = __p.__offset(12); return o != 0 ? __p.__vector_len(o) : 0; } }
        public FBS.Common.StringStringArray? MapProducerIdObserverIds(int j) { int o = __p.__offset(14); return o != 0 ? (FBS.Common.StringStringArray?)(new FBS.Common.StringStringArray()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int MapProducerIdObserverIdsLength { get { int o = __p.__offset(14); return o != 0 ? __p.__vector_len(o) : 0; } }
        public FBS.Common.StringStringArray? MapDataProducerIdDataConsumerIds(int j) { int o = __p.__offset(16); return o != 0 ? (FBS.Common.StringStringArray?)(new FBS.Common.StringStringArray()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int MapDataProducerIdDataConsumerIdsLength { get { int o = __p.__offset(16); return o != 0 ? __p.__vector_len(o) : 0; } }
        public FBS.Common.StringString? MapDataConsumerIdDataProducerId(int j) { int o = __p.__offset(18); return o != 0 ? (FBS.Common.StringString?)(new FBS.Common.StringString()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int MapDataConsumerIdDataProducerIdLength { get { int o = __p.__offset(18); return o != 0 ? __p.__vector_len(o) : 0; } }

        public static Offset<FBS.Router.DumpResponse> CreateDumpResponse(FlatBufferBuilder builder,
            StringOffset idOffset = default(StringOffset),
            VectorOffset transport_idsOffset = default(VectorOffset),
            VectorOffset rtp_observer_idsOffset = default(VectorOffset),
            VectorOffset map_producer_id_consumer_idsOffset = default(VectorOffset),
            VectorOffset map_consumer_id_producer_idOffset = default(VectorOffset),
            VectorOffset map_producer_id_observer_idsOffset = default(VectorOffset),
            VectorOffset map_data_producer_id_data_consumer_idsOffset = default(VectorOffset),
            VectorOffset map_data_consumer_id_data_producer_idOffset = default(VectorOffset))
        {
            builder.StartTable(8);
            DumpResponse.AddMapDataConsumerIdDataProducerId(builder, map_data_consumer_id_data_producer_idOffset);
            DumpResponse.AddMapDataProducerIdDataConsumerIds(builder, map_data_producer_id_data_consumer_idsOffset);
            DumpResponse.AddMapProducerIdObserverIds(builder, map_producer_id_observer_idsOffset);
            DumpResponse.AddMapConsumerIdProducerId(builder, map_consumer_id_producer_idOffset);
            DumpResponse.AddMapProducerIdConsumerIds(builder, map_producer_id_consumer_idsOffset);
            DumpResponse.AddRtpObserverIds(builder, rtp_observer_idsOffset);
            DumpResponse.AddTransportIds(builder, transport_idsOffset);
            DumpResponse.AddId(builder, idOffset);
            return DumpResponse.EndDumpResponse(builder);
        }

        public static void StartDumpResponse(FlatBufferBuilder builder) { builder.StartTable(8); }
        public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
        public static void AddTransportIds(FlatBufferBuilder builder, VectorOffset transportIdsOffset) { builder.AddOffset(1, transportIdsOffset.Value, 0); }
        public static VectorOffset CreateTransportIdsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateTransportIdsVectorBlock(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateTransportIdsVectorBlock(FlatBufferBuilder builder, ArraySegment<StringOffset> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateTransportIdsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<StringOffset>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartTransportIdsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddRtpObserverIds(FlatBufferBuilder builder, VectorOffset rtpObserverIdsOffset) { builder.AddOffset(2, rtpObserverIdsOffset.Value, 0); }
        public static VectorOffset CreateRtpObserverIdsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateRtpObserverIdsVectorBlock(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateRtpObserverIdsVectorBlock(FlatBufferBuilder builder, ArraySegment<StringOffset> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateRtpObserverIdsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<StringOffset>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartRtpObserverIdsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddMapProducerIdConsumerIds(FlatBufferBuilder builder, VectorOffset mapProducerIdConsumerIdsOffset) { builder.AddOffset(3, mapProducerIdConsumerIdsOffset.Value, 0); }
        public static VectorOffset CreateMapProducerIdConsumerIdsVector(FlatBufferBuilder builder, Offset<FBS.Common.StringStringArray>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateMapProducerIdConsumerIdsVectorBlock(FlatBufferBuilder builder, Offset<FBS.Common.StringStringArray>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateMapProducerIdConsumerIdsVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.Common.StringStringArray>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateMapProducerIdConsumerIdsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.Common.StringStringArray>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartMapProducerIdConsumerIdsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddMapConsumerIdProducerId(FlatBufferBuilder builder, VectorOffset mapConsumerIdProducerIdOffset) { builder.AddOffset(4, mapConsumerIdProducerIdOffset.Value, 0); }
        public static VectorOffset CreateMapConsumerIdProducerIdVector(FlatBufferBuilder builder, Offset<FBS.Common.StringString>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateMapConsumerIdProducerIdVectorBlock(FlatBufferBuilder builder, Offset<FBS.Common.StringString>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateMapConsumerIdProducerIdVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.Common.StringString>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateMapConsumerIdProducerIdVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.Common.StringString>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartMapConsumerIdProducerIdVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddMapProducerIdObserverIds(FlatBufferBuilder builder, VectorOffset mapProducerIdObserverIdsOffset) { builder.AddOffset(5, mapProducerIdObserverIdsOffset.Value, 0); }
        public static VectorOffset CreateMapProducerIdObserverIdsVector(FlatBufferBuilder builder, Offset<FBS.Common.StringStringArray>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateMapProducerIdObserverIdsVectorBlock(FlatBufferBuilder builder, Offset<FBS.Common.StringStringArray>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateMapProducerIdObserverIdsVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.Common.StringStringArray>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateMapProducerIdObserverIdsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.Common.StringStringArray>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartMapProducerIdObserverIdsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddMapDataProducerIdDataConsumerIds(FlatBufferBuilder builder, VectorOffset mapDataProducerIdDataConsumerIdsOffset) { builder.AddOffset(6, mapDataProducerIdDataConsumerIdsOffset.Value, 0); }
        public static VectorOffset CreateMapDataProducerIdDataConsumerIdsVector(FlatBufferBuilder builder, Offset<FBS.Common.StringStringArray>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateMapDataProducerIdDataConsumerIdsVectorBlock(FlatBufferBuilder builder, Offset<FBS.Common.StringStringArray>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateMapDataProducerIdDataConsumerIdsVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.Common.StringStringArray>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateMapDataProducerIdDataConsumerIdsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.Common.StringStringArray>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartMapDataProducerIdDataConsumerIdsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddMapDataConsumerIdDataProducerId(FlatBufferBuilder builder, VectorOffset mapDataConsumerIdDataProducerIdOffset) { builder.AddOffset(7, mapDataConsumerIdDataProducerIdOffset.Value, 0); }
        public static VectorOffset CreateMapDataConsumerIdDataProducerIdVector(FlatBufferBuilder builder, Offset<FBS.Common.StringString>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateMapDataConsumerIdDataProducerIdVectorBlock(FlatBufferBuilder builder, Offset<FBS.Common.StringString>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateMapDataConsumerIdDataProducerIdVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.Common.StringString>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateMapDataConsumerIdDataProducerIdVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.Common.StringString>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartMapDataConsumerIdDataProducerIdVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static Offset<FBS.Router.DumpResponse> EndDumpResponse(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // id
            builder.Required(o, 6);  // transport_ids
            builder.Required(o, 8);  // rtp_observer_ids
            builder.Required(o, 10);  // map_producer_id_consumer_ids
            builder.Required(o, 12);  // map_consumer_id_producer_id
            builder.Required(o, 14);  // map_producer_id_observer_ids
            builder.Required(o, 16);  // map_data_producer_id_data_consumer_ids
            builder.Required(o, 18);  // map_data_consumer_id_data_producer_id
            return new Offset<FBS.Router.DumpResponse>(o);
        }
        public DumpResponseT UnPack()
        {
            var _o = new DumpResponseT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(DumpResponseT _o)
        {
            _o.Id = this.Id;
            _o.TransportIds = new List<string>();
            for(var _j = 0; _j < this.TransportIdsLength; ++_j)
            { _o.TransportIds.Add(this.TransportIds(_j)); }
            _o.RtpObserverIds = new List<string>();
            for(var _j = 0; _j < this.RtpObserverIdsLength; ++_j)
            { _o.RtpObserverIds.Add(this.RtpObserverIds(_j)); }
            _o.MapProducerIdConsumerIds = new List<FBS.Common.StringStringArrayT>();
            for(var _j = 0; _j < this.MapProducerIdConsumerIdsLength; ++_j)
            { _o.MapProducerIdConsumerIds.Add(this.MapProducerIdConsumerIds(_j).HasValue ? this.MapProducerIdConsumerIds(_j).Value.UnPack() : null); }
            _o.MapConsumerIdProducerId = new List<FBS.Common.StringStringT>();
            for(var _j = 0; _j < this.MapConsumerIdProducerIdLength; ++_j)
            { _o.MapConsumerIdProducerId.Add(this.MapConsumerIdProducerId(_j).HasValue ? this.MapConsumerIdProducerId(_j).Value.UnPack() : null); }
            _o.MapProducerIdObserverIds = new List<FBS.Common.StringStringArrayT>();
            for(var _j = 0; _j < this.MapProducerIdObserverIdsLength; ++_j)
            { _o.MapProducerIdObserverIds.Add(this.MapProducerIdObserverIds(_j).HasValue ? this.MapProducerIdObserverIds(_j).Value.UnPack() : null); }
            _o.MapDataProducerIdDataConsumerIds = new List<FBS.Common.StringStringArrayT>();
            for(var _j = 0; _j < this.MapDataProducerIdDataConsumerIdsLength; ++_j)
            { _o.MapDataProducerIdDataConsumerIds.Add(this.MapDataProducerIdDataConsumerIds(_j).HasValue ? this.MapDataProducerIdDataConsumerIds(_j).Value.UnPack() : null); }
            _o.MapDataConsumerIdDataProducerId = new List<FBS.Common.StringStringT>();
            for(var _j = 0; _j < this.MapDataConsumerIdDataProducerIdLength; ++_j)
            { _o.MapDataConsumerIdDataProducerId.Add(this.MapDataConsumerIdDataProducerId(_j).HasValue ? this.MapDataConsumerIdDataProducerId(_j).Value.UnPack() : null); }
        }
        public static Offset<FBS.Router.DumpResponse> Pack(FlatBufferBuilder builder, DumpResponseT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Router.DumpResponse>);
            var _id = _o.Id == null ? default(StringOffset) : builder.CreateString(_o.Id);
            var _transport_ids = default(VectorOffset);
            if(_o.TransportIds != null)
            {
                var __transport_ids = new StringOffset[_o.TransportIds.Count];
                for(var _j = 0; _j < __transport_ids.Length; ++_j)
                { __transport_ids[_j] = builder.CreateString(_o.TransportIds[_j]); }
                _transport_ids = CreateTransportIdsVector(builder, __transport_ids);
            }
            var _rtp_observer_ids = default(VectorOffset);
            if(_o.RtpObserverIds != null)
            {
                var __rtp_observer_ids = new StringOffset[_o.RtpObserverIds.Count];
                for(var _j = 0; _j < __rtp_observer_ids.Length; ++_j)
                { __rtp_observer_ids[_j] = builder.CreateString(_o.RtpObserverIds[_j]); }
                _rtp_observer_ids = CreateRtpObserverIdsVector(builder, __rtp_observer_ids);
            }
            var _map_producer_id_consumer_ids = default(VectorOffset);
            if(_o.MapProducerIdConsumerIds != null)
            {
                var __map_producer_id_consumer_ids = new Offset<FBS.Common.StringStringArray>[_o.MapProducerIdConsumerIds.Count];
                for(var _j = 0; _j < __map_producer_id_consumer_ids.Length; ++_j)
                { __map_producer_id_consumer_ids[_j] = FBS.Common.StringStringArray.Pack(builder, _o.MapProducerIdConsumerIds[_j]); }
                _map_producer_id_consumer_ids = CreateMapProducerIdConsumerIdsVector(builder, __map_producer_id_consumer_ids);
            }
            var _map_consumer_id_producer_id = default(VectorOffset);
            if(_o.MapConsumerIdProducerId != null)
            {
                var __map_consumer_id_producer_id = new Offset<FBS.Common.StringString>[_o.MapConsumerIdProducerId.Count];
                for(var _j = 0; _j < __map_consumer_id_producer_id.Length; ++_j)
                { __map_consumer_id_producer_id[_j] = FBS.Common.StringString.Pack(builder, _o.MapConsumerIdProducerId[_j]); }
                _map_consumer_id_producer_id = CreateMapConsumerIdProducerIdVector(builder, __map_consumer_id_producer_id);
            }
            var _map_producer_id_observer_ids = default(VectorOffset);
            if(_o.MapProducerIdObserverIds != null)
            {
                var __map_producer_id_observer_ids = new Offset<FBS.Common.StringStringArray>[_o.MapProducerIdObserverIds.Count];
                for(var _j = 0; _j < __map_producer_id_observer_ids.Length; ++_j)
                { __map_producer_id_observer_ids[_j] = FBS.Common.StringStringArray.Pack(builder, _o.MapProducerIdObserverIds[_j]); }
                _map_producer_id_observer_ids = CreateMapProducerIdObserverIdsVector(builder, __map_producer_id_observer_ids);
            }
            var _map_data_producer_id_data_consumer_ids = default(VectorOffset);
            if(_o.MapDataProducerIdDataConsumerIds != null)
            {
                var __map_data_producer_id_data_consumer_ids = new Offset<FBS.Common.StringStringArray>[_o.MapDataProducerIdDataConsumerIds.Count];
                for(var _j = 0; _j < __map_data_producer_id_data_consumer_ids.Length; ++_j)
                { __map_data_producer_id_data_consumer_ids[_j] = FBS.Common.StringStringArray.Pack(builder, _o.MapDataProducerIdDataConsumerIds[_j]); }
                _map_data_producer_id_data_consumer_ids = CreateMapDataProducerIdDataConsumerIdsVector(builder, __map_data_producer_id_data_consumer_ids);
            }
            var _map_data_consumer_id_data_producer_id = default(VectorOffset);
            if(_o.MapDataConsumerIdDataProducerId != null)
            {
                var __map_data_consumer_id_data_producer_id = new Offset<FBS.Common.StringString>[_o.MapDataConsumerIdDataProducerId.Count];
                for(var _j = 0; _j < __map_data_consumer_id_data_producer_id.Length; ++_j)
                { __map_data_consumer_id_data_producer_id[_j] = FBS.Common.StringString.Pack(builder, _o.MapDataConsumerIdDataProducerId[_j]); }
                _map_data_consumer_id_data_producer_id = CreateMapDataConsumerIdDataProducerIdVector(builder, __map_data_consumer_id_data_producer_id);
            }
            return CreateDumpResponse(
              builder,
              _id,
              _transport_ids,
              _rtp_observer_ids,
              _map_producer_id_consumer_ids,
              _map_consumer_id_producer_id,
              _map_producer_id_observer_ids,
              _map_data_producer_id_data_consumer_ids,
              _map_data_consumer_id_data_producer_id);
        }
    }

    public class DumpResponseT
    {
        public string Id { get; set; }

        public List<string> TransportIds { get; set; }

        public List<string> RtpObserverIds { get; set; }

        public List<FBS.Common.StringStringArrayT> MapProducerIdConsumerIds { get; set; }

        public List<FBS.Common.StringStringT> MapConsumerIdProducerId { get; set; }

        public List<FBS.Common.StringStringArrayT> MapProducerIdObserverIds { get; set; }

        public List<FBS.Common.StringStringArrayT> MapDataProducerIdDataConsumerIds { get; set; }

        public List<FBS.Common.StringStringT> MapDataConsumerIdDataProducerId { get; set; }

        public DumpResponseT()
        {
            this.Id = null;
            this.TransportIds = null;
            this.RtpObserverIds = null;
            this.MapProducerIdConsumerIds = null;
            this.MapConsumerIdProducerId = null;
            this.MapProducerIdObserverIds = null;
            this.MapDataProducerIdDataConsumerIds = null;
            this.MapDataConsumerIdDataProducerId = null;
        }
    }


    static public class DumpResponseVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyString(tablePos, 4 /*Id*/, true)
              && verifier.VerifyVectorOfStrings(tablePos, 6 /*TransportIds*/, true)
              && verifier.VerifyVectorOfStrings(tablePos, 8 /*RtpObserverIds*/, true)
              && verifier.VerifyVectorOfTables(tablePos, 10 /*MapProducerIdConsumerIds*/, FBS.Common.StringStringArrayVerify.Verify, true)
              && verifier.VerifyVectorOfTables(tablePos, 12 /*MapConsumerIdProducerId*/, FBS.Common.StringStringVerify.Verify, true)
              && verifier.VerifyVectorOfTables(tablePos, 14 /*MapProducerIdObserverIds*/, FBS.Common.StringStringArrayVerify.Verify, true)
              && verifier.VerifyVectorOfTables(tablePos, 16 /*MapDataProducerIdDataConsumerIds*/, FBS.Common.StringStringArrayVerify.Verify, true)
              && verifier.VerifyVectorOfTables(tablePos, 18 /*MapDataConsumerIdDataProducerId*/, FBS.Common.StringStringVerify.Verify, true)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
