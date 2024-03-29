// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.DataConsumer
{
    public struct GetBufferedAmountResponse : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static GetBufferedAmountResponse GetRootAsGetBufferedAmountResponse(ByteBuffer _bb) { return GetRootAsGetBufferedAmountResponse(_bb, new GetBufferedAmountResponse()); }
        public static GetBufferedAmountResponse GetRootAsGetBufferedAmountResponse(ByteBuffer _bb, GetBufferedAmountResponse obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public GetBufferedAmountResponse __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public uint BufferedAmount { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }

        public static Offset<FBS.DataConsumer.GetBufferedAmountResponse> CreateGetBufferedAmountResponse(FlatBufferBuilder builder,
            uint buffered_amount = 0)
        {
            builder.StartTable(1);
            GetBufferedAmountResponse.AddBufferedAmount(builder, buffered_amount);
            return GetBufferedAmountResponse.EndGetBufferedAmountResponse(builder);
        }

        public static void StartGetBufferedAmountResponse(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddBufferedAmount(FlatBufferBuilder builder, uint bufferedAmount) { builder.AddUint(0, bufferedAmount, 0); }
        public static Offset<FBS.DataConsumer.GetBufferedAmountResponse> EndGetBufferedAmountResponse(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.DataConsumer.GetBufferedAmountResponse>(o);
        }
        public GetBufferedAmountResponseT UnPack()
        {
            var _o = new GetBufferedAmountResponseT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(GetBufferedAmountResponseT _o)
        {
            _o.BufferedAmount = this.BufferedAmount;
        }
        public static Offset<FBS.DataConsumer.GetBufferedAmountResponse> Pack(FlatBufferBuilder builder, GetBufferedAmountResponseT _o)
        {
            if(_o == null)
                return default(Offset<FBS.DataConsumer.GetBufferedAmountResponse>);
            return CreateGetBufferedAmountResponse(
              builder,
              _o.BufferedAmount);
        }
    }
}
