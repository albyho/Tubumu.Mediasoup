// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.Worker
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct CreateRouterRequest : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static CreateRouterRequest GetRootAsCreateRouterRequest(ByteBuffer _bb) { return GetRootAsCreateRouterRequest(_bb, new CreateRouterRequest()); }
        public static CreateRouterRequest GetRootAsCreateRouterRequest(ByteBuffer _bb, CreateRouterRequest obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public CreateRouterRequest __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string RouterId { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetRouterIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetRouterIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetRouterIdArray() { return __p.__vector_as_array<byte>(4); }

        public static Offset<FBS.Worker.CreateRouterRequest> CreateCreateRouterRequest(FlatBufferBuilder builder,
            StringOffset router_idOffset = default(StringOffset))
        {
            builder.StartTable(1);
            CreateRouterRequest.AddRouterId(builder, router_idOffset);
            return CreateRouterRequest.EndCreateRouterRequest(builder);
        }

        public static void StartCreateRouterRequest(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddRouterId(FlatBufferBuilder builder, StringOffset routerIdOffset) { builder.AddOffset(0, routerIdOffset.Value, 0); }
        public static Offset<FBS.Worker.CreateRouterRequest> EndCreateRouterRequest(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // router_id
            return new Offset<FBS.Worker.CreateRouterRequest>(o);
        }
        public CreateRouterRequestT UnPack()
        {
            var _o = new CreateRouterRequestT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(CreateRouterRequestT _o)
        {
            _o.RouterId = this.RouterId;
        }
        public static Offset<FBS.Worker.CreateRouterRequest> Pack(FlatBufferBuilder builder, CreateRouterRequestT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Worker.CreateRouterRequest>);
            var _router_id = _o.RouterId == null ? default(StringOffset) : builder.CreateString(_o.RouterId);
            return CreateCreateRouterRequest(
              builder,
              _router_id);
        }
    }

    public class CreateRouterRequestT
    {
        public string RouterId { get; set; }

        public CreateRouterRequestT()
        {
            this.RouterId = null;
        }
    }


    static public class CreateRouterRequestVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyString(tablePos, 4 /*RouterId*/, true)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
