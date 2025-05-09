// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.Worker
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct CloseRouterRequest : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static CloseRouterRequest GetRootAsCloseRouterRequest(ByteBuffer _bb) { return GetRootAsCloseRouterRequest(_bb, new CloseRouterRequest()); }
        public static CloseRouterRequest GetRootAsCloseRouterRequest(ByteBuffer _bb, CloseRouterRequest obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public CloseRouterRequest __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string RouterId { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetRouterIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetRouterIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetRouterIdArray() { return __p.__vector_as_array<byte>(4); }

        public static Offset<FBS.Worker.CloseRouterRequest> CreateCloseRouterRequest(FlatBufferBuilder builder,
            StringOffset router_idOffset = default(StringOffset))
        {
            builder.StartTable(1);
            CloseRouterRequest.AddRouterId(builder, router_idOffset);
            return CloseRouterRequest.EndCloseRouterRequest(builder);
        }

        public static void StartCloseRouterRequest(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddRouterId(FlatBufferBuilder builder, StringOffset routerIdOffset) { builder.AddOffset(0, routerIdOffset.Value, 0); }
        public static Offset<FBS.Worker.CloseRouterRequest> EndCloseRouterRequest(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // router_id
            return new Offset<FBS.Worker.CloseRouterRequest>(o);
        }
        public CloseRouterRequestT UnPack()
        {
            var _o = new CloseRouterRequestT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(CloseRouterRequestT _o)
        {
            _o.RouterId = this.RouterId;
        }
        public static Offset<FBS.Worker.CloseRouterRequest> Pack(FlatBufferBuilder builder, CloseRouterRequestT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Worker.CloseRouterRequest>);
            var _router_id = _o.RouterId == null ? default(StringOffset) : builder.CreateString(_o.RouterId);
            return CreateCloseRouterRequest(
              builder,
              _router_id);
        }
    }

    public class CloseRouterRequestT
    {
        public string RouterId { get; set; }

        public CloseRouterRequestT()
        {
            this.RouterId = null;
        }
    }


    static public class CloseRouterRequestVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyString(tablePos, 4 /*RouterId*/, true)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
