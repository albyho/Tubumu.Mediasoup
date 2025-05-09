// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.Consumer
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct SetPreferredLayersRequest : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static SetPreferredLayersRequest GetRootAsSetPreferredLayersRequest(ByteBuffer _bb) { return GetRootAsSetPreferredLayersRequest(_bb, new SetPreferredLayersRequest()); }
        public static SetPreferredLayersRequest GetRootAsSetPreferredLayersRequest(ByteBuffer _bb, SetPreferredLayersRequest obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public SetPreferredLayersRequest __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.Consumer.ConsumerLayers? PreferredLayers { get { int o = __p.__offset(4); return o != 0 ? (FBS.Consumer.ConsumerLayers?)(new FBS.Consumer.ConsumerLayers()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }

        public static Offset<FBS.Consumer.SetPreferredLayersRequest> CreateSetPreferredLayersRequest(FlatBufferBuilder builder,
            Offset<FBS.Consumer.ConsumerLayers> preferred_layersOffset = default(Offset<FBS.Consumer.ConsumerLayers>))
        {
            builder.StartTable(1);
            SetPreferredLayersRequest.AddPreferredLayers(builder, preferred_layersOffset);
            return SetPreferredLayersRequest.EndSetPreferredLayersRequest(builder);
        }

        public static void StartSetPreferredLayersRequest(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddPreferredLayers(FlatBufferBuilder builder, Offset<FBS.Consumer.ConsumerLayers> preferredLayersOffset) { builder.AddOffset(0, preferredLayersOffset.Value, 0); }
        public static Offset<FBS.Consumer.SetPreferredLayersRequest> EndSetPreferredLayersRequest(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // preferred_layers
            return new Offset<FBS.Consumer.SetPreferredLayersRequest>(o);
        }
        public SetPreferredLayersRequestT UnPack()
        {
            var _o = new SetPreferredLayersRequestT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(SetPreferredLayersRequestT _o)
        {
            _o.PreferredLayers = this.PreferredLayers.HasValue ? this.PreferredLayers.Value.UnPack() : null;
        }
        public static Offset<FBS.Consumer.SetPreferredLayersRequest> Pack(FlatBufferBuilder builder, SetPreferredLayersRequestT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Consumer.SetPreferredLayersRequest>);
            var _preferred_layers = _o.PreferredLayers == null ? default(Offset<FBS.Consumer.ConsumerLayers>) : FBS.Consumer.ConsumerLayers.Pack(builder, _o.PreferredLayers);
            return CreateSetPreferredLayersRequest(
              builder,
              _preferred_layers);
        }
    }

    public class SetPreferredLayersRequestT
    {
        public FBS.Consumer.ConsumerLayersT PreferredLayers { get; set; }

        public SetPreferredLayersRequestT()
        {
            this.PreferredLayers = null;
        }
    }


    static public class SetPreferredLayersRequestVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyTable(tablePos, 4 /*PreferredLayers*/, FBS.Consumer.ConsumerLayersVerify.Verify, true)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
