// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using System.Collections.Generic;
using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.Producer
{
    public struct SendNotification : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static SendNotification GetRootAsSendNotification(ByteBuffer _bb) { return GetRootAsSendNotification(_bb, new SendNotification()); }
        public static SendNotification GetRootAsSendNotification(ByteBuffer _bb, SendNotification obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public SendNotification __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public byte Data(int j) { int o = __p.__offset(4); return o != 0 ? __p.bb.Get(__p.__vector(o) + j * 1) : (byte)0; }
        public int DataLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }
#if ENABLE_SPAN_T
  public Span<byte> GetDataBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetDataBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetDataArray() { return __p.__vector_as_array<byte>(4); }

        public static Offset<FBS.Producer.SendNotification> CreateSendNotification(FlatBufferBuilder builder,
            VectorOffset dataOffset = default(VectorOffset))
        {
            builder.StartTable(1);
            SendNotification.AddData(builder, dataOffset);
            return SendNotification.EndSendNotification(builder);
        }

        public static void StartSendNotification(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddData(FlatBufferBuilder builder, VectorOffset dataOffset) { builder.AddOffset(0, dataOffset.Value, 0); }
        public static VectorOffset CreateDataVector(FlatBufferBuilder builder, byte[] data) { builder.StartVector(1, data.Length, 1); for(int i = data.Length - 1; i >= 0; i--) builder.AddByte(data[i]); return builder.EndVector(); }
        public static VectorOffset CreateDataVectorBlock(FlatBufferBuilder builder, byte[] data) { builder.StartVector(1, data.Length, 1); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateDataVectorBlock(FlatBufferBuilder builder, ArraySegment<byte> data) { builder.StartVector(1, data.Count, 1); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateDataVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<byte>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartDataVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(1, numElems, 1); }
        public static Offset<FBS.Producer.SendNotification> EndSendNotification(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // data
            return new Offset<FBS.Producer.SendNotification>(o);
        }
        public SendNotificationT UnPack()
        {
            var _o = new SendNotificationT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(SendNotificationT _o)
        {
            _o.Data = new List<byte>();
            for(var _j = 0; _j < this.DataLength; ++_j)
            { _o.Data.Add(this.Data(_j)); }
        }
        public static Offset<FBS.Producer.SendNotification> Pack(FlatBufferBuilder builder, SendNotificationT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Producer.SendNotification>);
            var _data = default(VectorOffset);
            if(_o.Data != null)
            {
                var __data = _o.Data.ToArray();
                _data = CreateDataVector(builder, __data);
            }
            return CreateSendNotification(
              builder,
              _data);
        }
    }

    public class SendNotificationT
    {
        [JsonPropertyName("data")]
        public List<byte> Data { get; set; }

        public SendNotificationT()
        {
            this.Data = null;
        }
    }
}