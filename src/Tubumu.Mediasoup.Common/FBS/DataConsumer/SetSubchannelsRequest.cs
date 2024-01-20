// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using System.Collections.Generic;
using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.DataConsumer
{
    public struct SetSubchannelsRequest : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static SetSubchannelsRequest GetRootAsSetSubchannelsRequest(ByteBuffer _bb) { return GetRootAsSetSubchannelsRequest(_bb, new SetSubchannelsRequest()); }
        public static SetSubchannelsRequest GetRootAsSetSubchannelsRequest(ByteBuffer _bb, SetSubchannelsRequest obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public SetSubchannelsRequest __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public ushort Subchannels(int j) { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUshort(__p.__vector(o) + j * 2) : (ushort)0; }
        public int SubchannelsLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }
#if ENABLE_SPAN_T
  public Span<ushort> GetSubchannelsBytes() { return __p.__vector_as_span<ushort>(4, 2); }
#else
        public ArraySegment<byte>? GetSubchannelsBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public ushort[] GetSubchannelsArray() { return __p.__vector_as_array<ushort>(4); }

        public static Offset<FBS.DataConsumer.SetSubchannelsRequest> CreateSetSubchannelsRequest(FlatBufferBuilder builder,
            VectorOffset subchannelsOffset = default(VectorOffset))
        {
            builder.StartTable(1);
            SetSubchannelsRequest.AddSubchannels(builder, subchannelsOffset);
            return SetSubchannelsRequest.EndSetSubchannelsRequest(builder);
        }

        public static void StartSetSubchannelsRequest(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddSubchannels(FlatBufferBuilder builder, VectorOffset subchannelsOffset) { builder.AddOffset(0, subchannelsOffset.Value, 0); }
        public static VectorOffset CreateSubchannelsVector(FlatBufferBuilder builder, ushort[] data) { builder.StartVector(2, data.Length, 2); for(int i = data.Length - 1; i >= 0; i--) builder.AddUshort(data[i]); return builder.EndVector(); }
        public static VectorOffset CreateSubchannelsVectorBlock(FlatBufferBuilder builder, ushort[] data) { builder.StartVector(2, data.Length, 2); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateSubchannelsVectorBlock(FlatBufferBuilder builder, ArraySegment<ushort> data) { builder.StartVector(2, data.Count, 2); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateSubchannelsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<ushort>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartSubchannelsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(2, numElems, 2); }
        public static Offset<FBS.DataConsumer.SetSubchannelsRequest> EndSetSubchannelsRequest(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // subchannels
            return new Offset<FBS.DataConsumer.SetSubchannelsRequest>(o);
        }
        public SetSubchannelsRequestT UnPack()
        {
            var _o = new SetSubchannelsRequestT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(SetSubchannelsRequestT _o)
        {
            _o.Subchannels = new List<ushort>();
            for(var _j = 0; _j < this.SubchannelsLength; ++_j)
            { _o.Subchannels.Add(this.Subchannels(_j)); }
        }
        public static Offset<FBS.DataConsumer.SetSubchannelsRequest> Pack(FlatBufferBuilder builder, SetSubchannelsRequestT _o)
        {
            if(_o == null)
                return default(Offset<FBS.DataConsumer.SetSubchannelsRequest>);
            var _subchannels = default(VectorOffset);
            if(_o.Subchannels != null)
            {
                var __subchannels = _o.Subchannels.ToArray();
                _subchannels = CreateSubchannelsVector(builder, __subchannels);
            }
            return CreateSetSubchannelsRequest(
              builder,
              _subchannels);
        }
    }

    public class SetSubchannelsRequestT
    {
        public List<ushort> Subchannels { get; set; }

        public SetSubchannelsRequestT()
        {
            this.Subchannels = null;
        }
    }
}
