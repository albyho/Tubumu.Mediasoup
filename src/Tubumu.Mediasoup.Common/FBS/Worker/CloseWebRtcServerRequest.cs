// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.Worker
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct CloseWebRtcServerRequest : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static CloseWebRtcServerRequest GetRootAsCloseWebRtcServerRequest(ByteBuffer _bb) { return GetRootAsCloseWebRtcServerRequest(_bb, new CloseWebRtcServerRequest()); }
        public static CloseWebRtcServerRequest GetRootAsCloseWebRtcServerRequest(ByteBuffer _bb, CloseWebRtcServerRequest obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public CloseWebRtcServerRequest __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string WebRtcServerId { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetWebRtcServerIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetWebRtcServerIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetWebRtcServerIdArray() { return __p.__vector_as_array<byte>(4); }

        public static Offset<FBS.Worker.CloseWebRtcServerRequest> CreateCloseWebRtcServerRequest(FlatBufferBuilder builder,
            StringOffset web_rtc_server_idOffset = default(StringOffset))
        {
            builder.StartTable(1);
            CloseWebRtcServerRequest.AddWebRtcServerId(builder, web_rtc_server_idOffset);
            return CloseWebRtcServerRequest.EndCloseWebRtcServerRequest(builder);
        }

        public static void StartCloseWebRtcServerRequest(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddWebRtcServerId(FlatBufferBuilder builder, StringOffset webRtcServerIdOffset) { builder.AddOffset(0, webRtcServerIdOffset.Value, 0); }
        public static Offset<FBS.Worker.CloseWebRtcServerRequest> EndCloseWebRtcServerRequest(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // web_rtc_server_id
            return new Offset<FBS.Worker.CloseWebRtcServerRequest>(o);
        }
        public CloseWebRtcServerRequestT UnPack()
        {
            var _o = new CloseWebRtcServerRequestT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(CloseWebRtcServerRequestT _o)
        {
            _o.WebRtcServerId = this.WebRtcServerId;
        }
        public static Offset<FBS.Worker.CloseWebRtcServerRequest> Pack(FlatBufferBuilder builder, CloseWebRtcServerRequestT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Worker.CloseWebRtcServerRequest>);
            var _web_rtc_server_id = _o.WebRtcServerId == null ? default(StringOffset) : builder.CreateString(_o.WebRtcServerId);
            return CreateCloseWebRtcServerRequest(
              builder,
              _web_rtc_server_id);
        }
    }

    public class CloseWebRtcServerRequestT
    {
        public string WebRtcServerId { get; set; }

        public CloseWebRtcServerRequestT()
        {
            this.WebRtcServerId = null;
        }
    }


    static public class CloseWebRtcServerRequestVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyString(tablePos, 4 /*WebRtcServerId*/, true)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
