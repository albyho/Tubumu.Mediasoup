// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.WebRtcTransport
{
    public struct ListenServer : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static ListenServer GetRootAsListenServer(ByteBuffer _bb) { return GetRootAsListenServer(_bb, new ListenServer()); }
        public static ListenServer GetRootAsListenServer(ByteBuffer _bb, ListenServer obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public ListenServer __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string WebRtcServerId { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetWebRtcServerIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetWebRtcServerIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetWebRtcServerIdArray() { return __p.__vector_as_array<byte>(4); }

        public static Offset<FBS.WebRtcTransport.ListenServer> CreateListenServer(FlatBufferBuilder builder,
            StringOffset web_rtc_server_idOffset = default(StringOffset))
        {
            builder.StartTable(1);
            ListenServer.AddWebRtcServerId(builder, web_rtc_server_idOffset);
            return ListenServer.EndListenServer(builder);
        }

        public static void StartListenServer(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddWebRtcServerId(FlatBufferBuilder builder, StringOffset webRtcServerIdOffset) { builder.AddOffset(0, webRtcServerIdOffset.Value, 0); }
        public static Offset<FBS.WebRtcTransport.ListenServer> EndListenServer(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // web_rtc_server_id
            return new Offset<FBS.WebRtcTransport.ListenServer>(o);
        }
        public ListenServerT UnPack()
        {
            var _o = new ListenServerT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(ListenServerT _o)
        {
            _o.WebRtcServerId = this.WebRtcServerId;
        }
        public static Offset<FBS.WebRtcTransport.ListenServer> Pack(FlatBufferBuilder builder, ListenServerT _o)
        {
            if(_o == null)
                return default(Offset<FBS.WebRtcTransport.ListenServer>);
            var _web_rtc_server_id = _o.WebRtcServerId == null ? default(StringOffset) : builder.CreateString(_o.WebRtcServerId);
            return CreateListenServer(
              builder,
              _web_rtc_server_id);
        }
    }

    public class ListenServerT
    {
        [JsonPropertyName("web_rtc_server_id")]
        public string WebRtcServerId { get; set; }

        public ListenServerT()
        {
            this.WebRtcServerId = null;
        }
    }
}
