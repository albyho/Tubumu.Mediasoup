// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.DataConsumer
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct RemoveSubchannelRequest : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static RemoveSubchannelRequest GetRootAsRemoveSubchannelRequest(ByteBuffer _bb) { return GetRootAsRemoveSubchannelRequest(_bb, new RemoveSubchannelRequest()); }
        public static RemoveSubchannelRequest GetRootAsRemoveSubchannelRequest(ByteBuffer _bb, RemoveSubchannelRequest obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public RemoveSubchannelRequest __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public ushort Subchannel { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUshort(o + __p.bb_pos) : (ushort)0; } }

        public static Offset<FBS.DataConsumer.RemoveSubchannelRequest> CreateRemoveSubchannelRequest(FlatBufferBuilder builder,
            ushort subchannel = 0)
        {
            builder.StartTable(1);
            RemoveSubchannelRequest.AddSubchannel(builder, subchannel);
            return RemoveSubchannelRequest.EndRemoveSubchannelRequest(builder);
        }

        public static void StartRemoveSubchannelRequest(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddSubchannel(FlatBufferBuilder builder, ushort subchannel) { builder.AddUshort(0, subchannel, 0); }
        public static Offset<FBS.DataConsumer.RemoveSubchannelRequest> EndRemoveSubchannelRequest(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.DataConsumer.RemoveSubchannelRequest>(o);
        }
        public RemoveSubchannelRequestT UnPack()
        {
            var _o = new RemoveSubchannelRequestT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(RemoveSubchannelRequestT _o)
        {
            _o.Subchannel = this.Subchannel;
        }
        public static Offset<FBS.DataConsumer.RemoveSubchannelRequest> Pack(FlatBufferBuilder builder, RemoveSubchannelRequestT _o)
        {
            if(_o == null)
                return default(Offset<FBS.DataConsumer.RemoveSubchannelRequest>);
            return CreateRemoveSubchannelRequest(
              builder,
              _o.Subchannel);
        }
    }

    public class RemoveSubchannelRequestT
    {
        public ushort Subchannel { get; set; }

        public RemoveSubchannelRequestT()
        {
            this.Subchannel = 0;
        }
    }


    static public class RemoveSubchannelRequestVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyField(tablePos, 4 /*Subchannel*/, 2 /*ushort*/, 2, false)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
