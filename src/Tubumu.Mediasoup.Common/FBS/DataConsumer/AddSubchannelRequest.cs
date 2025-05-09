// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.DataConsumer
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct AddSubchannelRequest : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static AddSubchannelRequest GetRootAsAddSubchannelRequest(ByteBuffer _bb) { return GetRootAsAddSubchannelRequest(_bb, new AddSubchannelRequest()); }
        public static AddSubchannelRequest GetRootAsAddSubchannelRequest(ByteBuffer _bb, AddSubchannelRequest obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public AddSubchannelRequest __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public ushort Subchannel { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUshort(o + __p.bb_pos) : (ushort)0; } }

        public static Offset<FBS.DataConsumer.AddSubchannelRequest> CreateAddSubchannelRequest(FlatBufferBuilder builder,
            ushort subchannel = 0)
        {
            builder.StartTable(1);
            AddSubchannelRequest.AddSubchannel(builder, subchannel);
            return AddSubchannelRequest.EndAddSubchannelRequest(builder);
        }

        public static void StartAddSubchannelRequest(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddSubchannel(FlatBufferBuilder builder, ushort subchannel) { builder.AddUshort(0, subchannel, 0); }
        public static Offset<FBS.DataConsumer.AddSubchannelRequest> EndAddSubchannelRequest(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.DataConsumer.AddSubchannelRequest>(o);
        }
        public AddSubchannelRequestT UnPack()
        {
            var _o = new AddSubchannelRequestT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(AddSubchannelRequestT _o)
        {
            _o.Subchannel = this.Subchannel;
        }
        public static Offset<FBS.DataConsumer.AddSubchannelRequest> Pack(FlatBufferBuilder builder, AddSubchannelRequestT _o)
        {
            if(_o == null)
                return default(Offset<FBS.DataConsumer.AddSubchannelRequest>);
            return CreateAddSubchannelRequest(
              builder,
              _o.Subchannel);
        }
    }

    public class AddSubchannelRequestT
    {
        public ushort Subchannel { get; set; }

        public AddSubchannelRequestT()
        {
            this.Subchannel = 0;
        }
    }


    static public class AddSubchannelRequestVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyField(tablePos, 4 /*Subchannel*/, 2 /*ushort*/, 2, false)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
