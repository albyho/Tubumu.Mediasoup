// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using System.Collections.Generic;
using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.Worker
{
    public struct CreateWebRtcServerRequest : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static CreateWebRtcServerRequest GetRootAsCreateWebRtcServerRequest(ByteBuffer _bb) { return GetRootAsCreateWebRtcServerRequest(_bb, new CreateWebRtcServerRequest()); }
        public static CreateWebRtcServerRequest GetRootAsCreateWebRtcServerRequest(ByteBuffer _bb, CreateWebRtcServerRequest obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public CreateWebRtcServerRequest __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string WebRtcServerId { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetWebRtcServerIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetWebRtcServerIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetWebRtcServerIdArray() { return __p.__vector_as_array<byte>(4); }
        public FBS.Transport.ListenInfo? ListenInfos(int j) { int o = __p.__offset(6); return o != 0 ? (FBS.Transport.ListenInfo?)(new FBS.Transport.ListenInfo()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int ListenInfosLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }

        public static Offset<FBS.Worker.CreateWebRtcServerRequest> CreateCreateWebRtcServerRequest(FlatBufferBuilder builder,
            StringOffset web_rtc_server_idOffset = default(StringOffset),
            VectorOffset listen_infosOffset = default(VectorOffset))
        {
            builder.StartTable(2);
            CreateWebRtcServerRequest.AddListenInfos(builder, listen_infosOffset);
            CreateWebRtcServerRequest.AddWebRtcServerId(builder, web_rtc_server_idOffset);
            return CreateWebRtcServerRequest.EndCreateWebRtcServerRequest(builder);
        }

        public static void StartCreateWebRtcServerRequest(FlatBufferBuilder builder) { builder.StartTable(2); }
        public static void AddWebRtcServerId(FlatBufferBuilder builder, StringOffset webRtcServerIdOffset) { builder.AddOffset(0, webRtcServerIdOffset.Value, 0); }
        public static void AddListenInfos(FlatBufferBuilder builder, VectorOffset listenInfosOffset) { builder.AddOffset(1, listenInfosOffset.Value, 0); }
        public static VectorOffset CreateListenInfosVector(FlatBufferBuilder builder, Offset<FBS.Transport.ListenInfo>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateListenInfosVectorBlock(FlatBufferBuilder builder, Offset<FBS.Transport.ListenInfo>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateListenInfosVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.Transport.ListenInfo>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateListenInfosVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.Transport.ListenInfo>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartListenInfosVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static Offset<FBS.Worker.CreateWebRtcServerRequest> EndCreateWebRtcServerRequest(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // web_rtc_server_id
            return new Offset<FBS.Worker.CreateWebRtcServerRequest>(o);
        }
        public CreateWebRtcServerRequestT UnPack()
        {
            var _o = new CreateWebRtcServerRequestT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(CreateWebRtcServerRequestT _o)
        {
            _o.WebRtcServerId = this.WebRtcServerId;
            _o.ListenInfos = new List<FBS.Transport.ListenInfoT>();
            for(var _j = 0; _j < this.ListenInfosLength; ++_j)
            { _o.ListenInfos.Add(this.ListenInfos(_j).HasValue ? this.ListenInfos(_j).Value.UnPack() : null); }
        }
        public static Offset<FBS.Worker.CreateWebRtcServerRequest> Pack(FlatBufferBuilder builder, CreateWebRtcServerRequestT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Worker.CreateWebRtcServerRequest>);
            var _web_rtc_server_id = _o.WebRtcServerId == null ? default(StringOffset) : builder.CreateString(_o.WebRtcServerId);
            var _listen_infos = default(VectorOffset);
            if(_o.ListenInfos != null)
            {
                var __listen_infos = new Offset<FBS.Transport.ListenInfo>[_o.ListenInfos.Count];
                for(var _j = 0; _j < __listen_infos.Length; ++_j)
                { __listen_infos[_j] = FBS.Transport.ListenInfo.Pack(builder, _o.ListenInfos[_j]); }
                _listen_infos = CreateListenInfosVector(builder, __listen_infos);
            }
            return CreateCreateWebRtcServerRequest(
              builder,
              _web_rtc_server_id,
              _listen_infos);
        }
    }
}
