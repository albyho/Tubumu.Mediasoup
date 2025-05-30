// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.Transport
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct Dump : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static Dump GetRootAsDump(ByteBuffer _bb) { return GetRootAsDump(_bb, new Dump()); }
        public static Dump GetRootAsDump(ByteBuffer _bb, Dump obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public Dump __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string Id { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetIdArray() { return __p.__vector_as_array<byte>(4); }
        public bool Direct { get { int o = __p.__offset(6); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }
        public string ProducerIds(int j) { int o = __p.__offset(8); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : null; }
        public int ProducerIdsLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
        public string ConsumerIds(int j) { int o = __p.__offset(10); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : null; }
        public int ConsumerIdsLength { get { int o = __p.__offset(10); return o != 0 ? __p.__vector_len(o) : 0; } }
        public FBS.Common.Uint32String? MapSsrcConsumerId(int j) { int o = __p.__offset(12); return o != 0 ? (FBS.Common.Uint32String?)(new FBS.Common.Uint32String()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int MapSsrcConsumerIdLength { get { int o = __p.__offset(12); return o != 0 ? __p.__vector_len(o) : 0; } }
        public FBS.Common.Uint32String? MapRtxSsrcConsumerId(int j) { int o = __p.__offset(14); return o != 0 ? (FBS.Common.Uint32String?)(new FBS.Common.Uint32String()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int MapRtxSsrcConsumerIdLength { get { int o = __p.__offset(14); return o != 0 ? __p.__vector_len(o) : 0; } }
        public string DataProducerIds(int j) { int o = __p.__offset(16); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : null; }
        public int DataProducerIdsLength { get { int o = __p.__offset(16); return o != 0 ? __p.__vector_len(o) : 0; } }
        public string DataConsumerIds(int j) { int o = __p.__offset(18); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : null; }
        public int DataConsumerIdsLength { get { int o = __p.__offset(18); return o != 0 ? __p.__vector_len(o) : 0; } }
        public FBS.Transport.RecvRtpHeaderExtensions? RecvRtpHeaderExtensions { get { int o = __p.__offset(20); return o != 0 ? (FBS.Transport.RecvRtpHeaderExtensions?)(new FBS.Transport.RecvRtpHeaderExtensions()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public FBS.Transport.RtpListener? RtpListener { get { int o = __p.__offset(22); return o != 0 ? (FBS.Transport.RtpListener?)(new FBS.Transport.RtpListener()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public uint MaxMessageSize { get { int o = __p.__offset(24); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public FBS.SctpParameters.SctpParameters? SctpParameters { get { int o = __p.__offset(26); return o != 0 ? (FBS.SctpParameters.SctpParameters?)(new FBS.SctpParameters.SctpParameters()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public FBS.SctpAssociation.SctpState? SctpState { get { int o = __p.__offset(28); return o != 0 ? (FBS.SctpAssociation.SctpState)__p.bb.Get(o + __p.bb_pos) : (FBS.SctpAssociation.SctpState?)null; } }
        public FBS.Transport.SctpListener? SctpListener { get { int o = __p.__offset(30); return o != 0 ? (FBS.Transport.SctpListener?)(new FBS.Transport.SctpListener()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public FBS.Transport.TraceEventType TraceEventTypes(int j) { int o = __p.__offset(32); return o != 0 ? (FBS.Transport.TraceEventType)__p.bb.Get(__p.__vector(o) + j * 1) : (FBS.Transport.TraceEventType)0; }
        public int TraceEventTypesLength { get { int o = __p.__offset(32); return o != 0 ? __p.__vector_len(o) : 0; } }
#if ENABLE_SPAN_T
  public Span<FBS.Transport.TraceEventType> GetTraceEventTypesBytes() { return __p.__vector_as_span<FBS.Transport.TraceEventType>(32, 1); }
#else
        public ArraySegment<byte>? GetTraceEventTypesBytes() { return __p.__vector_as_arraysegment(32); }
#endif
        public FBS.Transport.TraceEventType[] GetTraceEventTypesArray() { int o = __p.__offset(32); if(o == 0) return null; int p = __p.__vector(o); int l = __p.__vector_len(o); FBS.Transport.TraceEventType[] a = new FBS.Transport.TraceEventType[l]; for(int i = 0; i < l; i++) { a[i] = (FBS.Transport.TraceEventType)__p.bb.Get(p + i * 1); } return a; }

        public static Offset<FBS.Transport.Dump> CreateDump(FlatBufferBuilder builder,
            StringOffset idOffset = default(StringOffset),
            bool direct = false,
            VectorOffset producer_idsOffset = default(VectorOffset),
            VectorOffset consumer_idsOffset = default(VectorOffset),
            VectorOffset map_ssrc_consumer_idOffset = default(VectorOffset),
            VectorOffset map_rtx_ssrc_consumer_idOffset = default(VectorOffset),
            VectorOffset data_producer_idsOffset = default(VectorOffset),
            VectorOffset data_consumer_idsOffset = default(VectorOffset),
            Offset<FBS.Transport.RecvRtpHeaderExtensions> recv_rtp_header_extensionsOffset = default(Offset<FBS.Transport.RecvRtpHeaderExtensions>),
            Offset<FBS.Transport.RtpListener> rtp_listenerOffset = default(Offset<FBS.Transport.RtpListener>),
            uint max_message_size = 0,
            Offset<FBS.SctpParameters.SctpParameters> sctp_parametersOffset = default(Offset<FBS.SctpParameters.SctpParameters>),
            FBS.SctpAssociation.SctpState? sctp_state = null,
            Offset<FBS.Transport.SctpListener> sctp_listenerOffset = default(Offset<FBS.Transport.SctpListener>),
            VectorOffset trace_event_typesOffset = default(VectorOffset))
        {
            builder.StartTable(15);
            Dump.AddTraceEventTypes(builder, trace_event_typesOffset);
            Dump.AddSctpListener(builder, sctp_listenerOffset);
            Dump.AddSctpParameters(builder, sctp_parametersOffset);
            Dump.AddMaxMessageSize(builder, max_message_size);
            Dump.AddRtpListener(builder, rtp_listenerOffset);
            Dump.AddRecvRtpHeaderExtensions(builder, recv_rtp_header_extensionsOffset);
            Dump.AddDataConsumerIds(builder, data_consumer_idsOffset);
            Dump.AddDataProducerIds(builder, data_producer_idsOffset);
            Dump.AddMapRtxSsrcConsumerId(builder, map_rtx_ssrc_consumer_idOffset);
            Dump.AddMapSsrcConsumerId(builder, map_ssrc_consumer_idOffset);
            Dump.AddConsumerIds(builder, consumer_idsOffset);
            Dump.AddProducerIds(builder, producer_idsOffset);
            Dump.AddId(builder, idOffset);
            Dump.AddSctpState(builder, sctp_state);
            Dump.AddDirect(builder, direct);
            return Dump.EndDump(builder);
        }

        public static void StartDump(FlatBufferBuilder builder) { builder.StartTable(15); }
        public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
        public static void AddDirect(FlatBufferBuilder builder, bool direct) { builder.AddBool(1, direct, false); }
        public static void AddProducerIds(FlatBufferBuilder builder, VectorOffset producerIdsOffset) { builder.AddOffset(2, producerIdsOffset.Value, 0); }
        public static VectorOffset CreateProducerIdsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateProducerIdsVectorBlock(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateProducerIdsVectorBlock(FlatBufferBuilder builder, ArraySegment<StringOffset> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateProducerIdsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<StringOffset>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartProducerIdsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddConsumerIds(FlatBufferBuilder builder, VectorOffset consumerIdsOffset) { builder.AddOffset(3, consumerIdsOffset.Value, 0); }
        public static VectorOffset CreateConsumerIdsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateConsumerIdsVectorBlock(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateConsumerIdsVectorBlock(FlatBufferBuilder builder, ArraySegment<StringOffset> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateConsumerIdsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<StringOffset>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartConsumerIdsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddMapSsrcConsumerId(FlatBufferBuilder builder, VectorOffset mapSsrcConsumerIdOffset) { builder.AddOffset(4, mapSsrcConsumerIdOffset.Value, 0); }
        public static VectorOffset CreateMapSsrcConsumerIdVector(FlatBufferBuilder builder, Offset<FBS.Common.Uint32String>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateMapSsrcConsumerIdVectorBlock(FlatBufferBuilder builder, Offset<FBS.Common.Uint32String>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateMapSsrcConsumerIdVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.Common.Uint32String>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateMapSsrcConsumerIdVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.Common.Uint32String>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartMapSsrcConsumerIdVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddMapRtxSsrcConsumerId(FlatBufferBuilder builder, VectorOffset mapRtxSsrcConsumerIdOffset) { builder.AddOffset(5, mapRtxSsrcConsumerIdOffset.Value, 0); }
        public static VectorOffset CreateMapRtxSsrcConsumerIdVector(FlatBufferBuilder builder, Offset<FBS.Common.Uint32String>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateMapRtxSsrcConsumerIdVectorBlock(FlatBufferBuilder builder, Offset<FBS.Common.Uint32String>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateMapRtxSsrcConsumerIdVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.Common.Uint32String>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateMapRtxSsrcConsumerIdVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.Common.Uint32String>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartMapRtxSsrcConsumerIdVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddDataProducerIds(FlatBufferBuilder builder, VectorOffset dataProducerIdsOffset) { builder.AddOffset(6, dataProducerIdsOffset.Value, 0); }
        public static VectorOffset CreateDataProducerIdsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateDataProducerIdsVectorBlock(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateDataProducerIdsVectorBlock(FlatBufferBuilder builder, ArraySegment<StringOffset> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateDataProducerIdsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<StringOffset>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartDataProducerIdsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddDataConsumerIds(FlatBufferBuilder builder, VectorOffset dataConsumerIdsOffset) { builder.AddOffset(7, dataConsumerIdsOffset.Value, 0); }
        public static VectorOffset CreateDataConsumerIdsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateDataConsumerIdsVectorBlock(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateDataConsumerIdsVectorBlock(FlatBufferBuilder builder, ArraySegment<StringOffset> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateDataConsumerIdsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<StringOffset>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartDataConsumerIdsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddRecvRtpHeaderExtensions(FlatBufferBuilder builder, Offset<FBS.Transport.RecvRtpHeaderExtensions> recvRtpHeaderExtensionsOffset) { builder.AddOffset(8, recvRtpHeaderExtensionsOffset.Value, 0); }
        public static void AddRtpListener(FlatBufferBuilder builder, Offset<FBS.Transport.RtpListener> rtpListenerOffset) { builder.AddOffset(9, rtpListenerOffset.Value, 0); }
        public static void AddMaxMessageSize(FlatBufferBuilder builder, uint maxMessageSize) { builder.AddUint(10, maxMessageSize, 0); }
        public static void AddSctpParameters(FlatBufferBuilder builder, Offset<FBS.SctpParameters.SctpParameters> sctpParametersOffset) { builder.AddOffset(11, sctpParametersOffset.Value, 0); }
        public static void AddSctpState(FlatBufferBuilder builder, FBS.SctpAssociation.SctpState? sctpState) { builder.AddByte(12, (byte?)sctpState); }
        public static void AddSctpListener(FlatBufferBuilder builder, Offset<FBS.Transport.SctpListener> sctpListenerOffset) { builder.AddOffset(13, sctpListenerOffset.Value, 0); }
        public static void AddTraceEventTypes(FlatBufferBuilder builder, VectorOffset traceEventTypesOffset) { builder.AddOffset(14, traceEventTypesOffset.Value, 0); }
        public static VectorOffset CreateTraceEventTypesVector(FlatBufferBuilder builder, FBS.Transport.TraceEventType[] data) { builder.StartVector(1, data.Length, 1); for(int i = data.Length - 1; i >= 0; i--) builder.AddByte((byte)data[i]); return builder.EndVector(); }
        public static VectorOffset CreateTraceEventTypesVectorBlock(FlatBufferBuilder builder, FBS.Transport.TraceEventType[] data) { builder.StartVector(1, data.Length, 1); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateTraceEventTypesVectorBlock(FlatBufferBuilder builder, ArraySegment<FBS.Transport.TraceEventType> data) { builder.StartVector(1, data.Count, 1); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateTraceEventTypesVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<FBS.Transport.TraceEventType>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartTraceEventTypesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(1, numElems, 1); }
        public static Offset<FBS.Transport.Dump> EndDump(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // id
            builder.Required(o, 8);  // producer_ids
            builder.Required(o, 10);  // consumer_ids
            builder.Required(o, 12);  // map_ssrc_consumer_id
            builder.Required(o, 14);  // map_rtx_ssrc_consumer_id
            builder.Required(o, 16);  // data_producer_ids
            builder.Required(o, 18);  // data_consumer_ids
            builder.Required(o, 20);  // recv_rtp_header_extensions
            builder.Required(o, 22);  // rtp_listener
            builder.Required(o, 32);  // trace_event_types
            return new Offset<FBS.Transport.Dump>(o);
        }
        public DumpT UnPack()
        {
            var _o = new DumpT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(DumpT _o)
        {
            _o.Id = this.Id;
            _o.Direct = this.Direct;
            _o.ProducerIds = new List<string>();
            for(var _j = 0; _j < this.ProducerIdsLength; ++_j)
            { _o.ProducerIds.Add(this.ProducerIds(_j)); }
            _o.ConsumerIds = new List<string>();
            for(var _j = 0; _j < this.ConsumerIdsLength; ++_j)
            { _o.ConsumerIds.Add(this.ConsumerIds(_j)); }
            _o.MapSsrcConsumerId = new List<FBS.Common.Uint32StringT>();
            for(var _j = 0; _j < this.MapSsrcConsumerIdLength; ++_j)
            { _o.MapSsrcConsumerId.Add(this.MapSsrcConsumerId(_j).HasValue ? this.MapSsrcConsumerId(_j).Value.UnPack() : null); }
            _o.MapRtxSsrcConsumerId = new List<FBS.Common.Uint32StringT>();
            for(var _j = 0; _j < this.MapRtxSsrcConsumerIdLength; ++_j)
            { _o.MapRtxSsrcConsumerId.Add(this.MapRtxSsrcConsumerId(_j).HasValue ? this.MapRtxSsrcConsumerId(_j).Value.UnPack() : null); }
            _o.DataProducerIds = new List<string>();
            for(var _j = 0; _j < this.DataProducerIdsLength; ++_j)
            { _o.DataProducerIds.Add(this.DataProducerIds(_j)); }
            _o.DataConsumerIds = new List<string>();
            for(var _j = 0; _j < this.DataConsumerIdsLength; ++_j)
            { _o.DataConsumerIds.Add(this.DataConsumerIds(_j)); }
            _o.RecvRtpHeaderExtensions = this.RecvRtpHeaderExtensions.HasValue ? this.RecvRtpHeaderExtensions.Value.UnPack() : null;
            _o.RtpListener = this.RtpListener.HasValue ? this.RtpListener.Value.UnPack() : null;
            _o.MaxMessageSize = this.MaxMessageSize;
            _o.SctpParameters = this.SctpParameters.HasValue ? this.SctpParameters.Value.UnPack() : null;
            _o.SctpState = this.SctpState;
            _o.SctpListener = this.SctpListener.HasValue ? this.SctpListener.Value.UnPack() : null;
            _o.TraceEventTypes = new List<FBS.Transport.TraceEventType>();
            for(var _j = 0; _j < this.TraceEventTypesLength; ++_j)
            { _o.TraceEventTypes.Add(this.TraceEventTypes(_j)); }
        }
        public static Offset<FBS.Transport.Dump> Pack(FlatBufferBuilder builder, DumpT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Transport.Dump>);
            var _id = _o.Id == null ? default(StringOffset) : builder.CreateString(_o.Id);
            var _producer_ids = default(VectorOffset);
            if(_o.ProducerIds != null)
            {
                var __producer_ids = new StringOffset[_o.ProducerIds.Count];
                for(var _j = 0; _j < __producer_ids.Length; ++_j)
                { __producer_ids[_j] = builder.CreateString(_o.ProducerIds[_j]); }
                _producer_ids = CreateProducerIdsVector(builder, __producer_ids);
            }
            var _consumer_ids = default(VectorOffset);
            if(_o.ConsumerIds != null)
            {
                var __consumer_ids = new StringOffset[_o.ConsumerIds.Count];
                for(var _j = 0; _j < __consumer_ids.Length; ++_j)
                { __consumer_ids[_j] = builder.CreateString(_o.ConsumerIds[_j]); }
                _consumer_ids = CreateConsumerIdsVector(builder, __consumer_ids);
            }
            var _map_ssrc_consumer_id = default(VectorOffset);
            if(_o.MapSsrcConsumerId != null)
            {
                var __map_ssrc_consumer_id = new Offset<FBS.Common.Uint32String>[_o.MapSsrcConsumerId.Count];
                for(var _j = 0; _j < __map_ssrc_consumer_id.Length; ++_j)
                { __map_ssrc_consumer_id[_j] = FBS.Common.Uint32String.Pack(builder, _o.MapSsrcConsumerId[_j]); }
                _map_ssrc_consumer_id = CreateMapSsrcConsumerIdVector(builder, __map_ssrc_consumer_id);
            }
            var _map_rtx_ssrc_consumer_id = default(VectorOffset);
            if(_o.MapRtxSsrcConsumerId != null)
            {
                var __map_rtx_ssrc_consumer_id = new Offset<FBS.Common.Uint32String>[_o.MapRtxSsrcConsumerId.Count];
                for(var _j = 0; _j < __map_rtx_ssrc_consumer_id.Length; ++_j)
                { __map_rtx_ssrc_consumer_id[_j] = FBS.Common.Uint32String.Pack(builder, _o.MapRtxSsrcConsumerId[_j]); }
                _map_rtx_ssrc_consumer_id = CreateMapRtxSsrcConsumerIdVector(builder, __map_rtx_ssrc_consumer_id);
            }
            var _data_producer_ids = default(VectorOffset);
            if(_o.DataProducerIds != null)
            {
                var __data_producer_ids = new StringOffset[_o.DataProducerIds.Count];
                for(var _j = 0; _j < __data_producer_ids.Length; ++_j)
                { __data_producer_ids[_j] = builder.CreateString(_o.DataProducerIds[_j]); }
                _data_producer_ids = CreateDataProducerIdsVector(builder, __data_producer_ids);
            }
            var _data_consumer_ids = default(VectorOffset);
            if(_o.DataConsumerIds != null)
            {
                var __data_consumer_ids = new StringOffset[_o.DataConsumerIds.Count];
                for(var _j = 0; _j < __data_consumer_ids.Length; ++_j)
                { __data_consumer_ids[_j] = builder.CreateString(_o.DataConsumerIds[_j]); }
                _data_consumer_ids = CreateDataConsumerIdsVector(builder, __data_consumer_ids);
            }
            var _recv_rtp_header_extensions = _o.RecvRtpHeaderExtensions == null ? default(Offset<FBS.Transport.RecvRtpHeaderExtensions>) : FBS.Transport.RecvRtpHeaderExtensions.Pack(builder, _o.RecvRtpHeaderExtensions);
            var _rtp_listener = _o.RtpListener == null ? default(Offset<FBS.Transport.RtpListener>) : FBS.Transport.RtpListener.Pack(builder, _o.RtpListener);
            var _sctp_parameters = _o.SctpParameters == null ? default(Offset<FBS.SctpParameters.SctpParameters>) : FBS.SctpParameters.SctpParameters.Pack(builder, _o.SctpParameters);
            var _sctp_listener = _o.SctpListener == null ? default(Offset<FBS.Transport.SctpListener>) : FBS.Transport.SctpListener.Pack(builder, _o.SctpListener);
            var _trace_event_types = default(VectorOffset);
            if(_o.TraceEventTypes != null)
            {
                var __trace_event_types = _o.TraceEventTypes.ToArray();
                _trace_event_types = CreateTraceEventTypesVector(builder, __trace_event_types);
            }
            return CreateDump(
              builder,
              _id,
              _o.Direct,
              _producer_ids,
              _consumer_ids,
              _map_ssrc_consumer_id,
              _map_rtx_ssrc_consumer_id,
              _data_producer_ids,
              _data_consumer_ids,
              _recv_rtp_header_extensions,
              _rtp_listener,
              _o.MaxMessageSize,
              _sctp_parameters,
              _o.SctpState,
              _sctp_listener,
              _trace_event_types);
        }
    }

    public class DumpT
    {
        public string Id { get; set; }

        public bool Direct { get; set; }

        public List<string> ProducerIds { get; set; }

        public List<string> ConsumerIds { get; set; }

        public List<FBS.Common.Uint32StringT> MapSsrcConsumerId { get; set; }

        public List<FBS.Common.Uint32StringT> MapRtxSsrcConsumerId { get; set; }

        public List<string> DataProducerIds { get; set; }

        public List<string> DataConsumerIds { get; set; }

        public FBS.Transport.RecvRtpHeaderExtensionsT RecvRtpHeaderExtensions { get; set; }

        public FBS.Transport.RtpListenerT RtpListener { get; set; }

        public uint MaxMessageSize { get; set; }

        public FBS.SctpParameters.SctpParametersT SctpParameters { get; set; }

        public FBS.SctpAssociation.SctpState? SctpState { get; set; }

        public FBS.Transport.SctpListenerT SctpListener { get; set; }

        public List<FBS.Transport.TraceEventType> TraceEventTypes { get; set; }

        public DumpT()
        {
            this.Id = null;
            this.Direct = false;
            this.ProducerIds = null;
            this.ConsumerIds = null;
            this.MapSsrcConsumerId = null;
            this.MapRtxSsrcConsumerId = null;
            this.DataProducerIds = null;
            this.DataConsumerIds = null;
            this.RecvRtpHeaderExtensions = null;
            this.RtpListener = null;
            this.MaxMessageSize = 0;
            this.SctpParameters = null;
            this.SctpState = null;
            this.SctpListener = null;
            this.TraceEventTypes = null;
        }
    }


    static public class DumpVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyString(tablePos, 4 /*Id*/, true)
              && verifier.VerifyField(tablePos, 6 /*Direct*/, 1 /*bool*/, 1, false)
              && verifier.VerifyVectorOfStrings(tablePos, 8 /*ProducerIds*/, true)
              && verifier.VerifyVectorOfStrings(tablePos, 10 /*ConsumerIds*/, true)
              && verifier.VerifyVectorOfTables(tablePos, 12 /*MapSsrcConsumerId*/, FBS.Common.Uint32StringVerify.Verify, true)
              && verifier.VerifyVectorOfTables(tablePos, 14 /*MapRtxSsrcConsumerId*/, FBS.Common.Uint32StringVerify.Verify, true)
              && verifier.VerifyVectorOfStrings(tablePos, 16 /*DataProducerIds*/, true)
              && verifier.VerifyVectorOfStrings(tablePos, 18 /*DataConsumerIds*/, true)
              && verifier.VerifyTable(tablePos, 20 /*RecvRtpHeaderExtensions*/, FBS.Transport.RecvRtpHeaderExtensionsVerify.Verify, true)
              && verifier.VerifyTable(tablePos, 22 /*RtpListener*/, FBS.Transport.RtpListenerVerify.Verify, true)
              && verifier.VerifyField(tablePos, 24 /*MaxMessageSize*/, 4 /*uint*/, 4, false)
              && verifier.VerifyTable(tablePos, 26 /*SctpParameters*/, FBS.SctpParameters.SctpParametersVerify.Verify, false)
              && verifier.VerifyField(tablePos, 28 /*SctpState*/, 1 /*FBS.SctpAssociation.SctpState*/, 1, false)
              && verifier.VerifyTable(tablePos, 30 /*SctpListener*/, FBS.Transport.SctpListenerVerify.Verify, false)
              && verifier.VerifyVectorOfData(tablePos, 32 /*TraceEventTypes*/, 1 /*FBS.Transport.TraceEventType*/, true)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
