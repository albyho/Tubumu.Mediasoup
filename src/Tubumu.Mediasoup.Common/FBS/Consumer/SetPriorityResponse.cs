// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.Consumer
{
    public struct SetPriorityResponse : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static SetPriorityResponse GetRootAsSetPriorityResponse(ByteBuffer _bb) { return GetRootAsSetPriorityResponse(_bb, new SetPriorityResponse()); }
        public static SetPriorityResponse GetRootAsSetPriorityResponse(ByteBuffer _bb, SetPriorityResponse obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public SetPriorityResponse __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public byte Priority { get { int o = __p.__offset(4); return o != 0 ? __p.bb.Get(o + __p.bb_pos) : (byte)0; } }

        public static Offset<FBS.Consumer.SetPriorityResponse> CreateSetPriorityResponse(FlatBufferBuilder builder,
            byte priority = 0)
        {
            builder.StartTable(1);
            SetPriorityResponse.AddPriority(builder, priority);
            return SetPriorityResponse.EndSetPriorityResponse(builder);
        }

        public static void StartSetPriorityResponse(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddPriority(FlatBufferBuilder builder, byte priority) { builder.AddByte(0, priority, 0); }
        public static Offset<FBS.Consumer.SetPriorityResponse> EndSetPriorityResponse(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.Consumer.SetPriorityResponse>(o);
        }
        public SetPriorityResponseT UnPack()
        {
            var _o = new SetPriorityResponseT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(SetPriorityResponseT _o)
        {
            _o.Priority = this.Priority;
        }
        public static Offset<FBS.Consumer.SetPriorityResponse> Pack(FlatBufferBuilder builder, SetPriorityResponseT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Consumer.SetPriorityResponse>);
            return CreateSetPriorityResponse(
              builder,
              _o.Priority);
        }
    }
}
