// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using System.Collections.Generic;
using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.DataConsumer
{
    public struct MessageNotification : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static MessageNotification GetRootAsMessageNotification(ByteBuffer _bb) { return GetRootAsMessageNotification(_bb, new MessageNotification()); }
        public static MessageNotification GetRootAsMessageNotification(ByteBuffer _bb, MessageNotification obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public MessageNotification __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public uint Ppid { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public byte Data(int j) { int o = __p.__offset(6); return o != 0 ? __p.bb.Get(__p.__vector(o) + j * 1) : (byte)0; }
        public int DataLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }
#if ENABLE_SPAN_T
  public Span<byte> GetDataBytes() { return __p.__vector_as_span<byte>(6, 1); }
#else
        public ArraySegment<byte>? GetDataBytes() { return __p.__vector_as_arraysegment(6); }
#endif
        public byte[] GetDataArray() { return __p.__vector_as_array<byte>(6); }

        public static Offset<FBS.DataConsumer.MessageNotification> CreateMessageNotification(FlatBufferBuilder builder,
            uint ppid = 0,
            VectorOffset dataOffset = default(VectorOffset))
        {
            builder.StartTable(2);
            MessageNotification.AddData(builder, dataOffset);
            MessageNotification.AddPpid(builder, ppid);
            return MessageNotification.EndMessageNotification(builder);
        }

        public static void StartMessageNotification(FlatBufferBuilder builder) { builder.StartTable(2); }
        public static void AddPpid(FlatBufferBuilder builder, uint ppid) { builder.AddUint(0, ppid, 0); }
        public static void AddData(FlatBufferBuilder builder, VectorOffset dataOffset) { builder.AddOffset(1, dataOffset.Value, 0); }
        public static VectorOffset CreateDataVector(FlatBufferBuilder builder, byte[] data) { builder.StartVector(1, data.Length, 1); for(int i = data.Length - 1; i >= 0; i--) builder.AddByte(data[i]); return builder.EndVector(); }
        public static VectorOffset CreateDataVectorBlock(FlatBufferBuilder builder, byte[] data) { builder.StartVector(1, data.Length, 1); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateDataVectorBlock(FlatBufferBuilder builder, ArraySegment<byte> data) { builder.StartVector(1, data.Count, 1); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateDataVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<byte>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartDataVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(1, numElems, 1); }
        public static Offset<FBS.DataConsumer.MessageNotification> EndMessageNotification(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 6);  // data
            return new Offset<FBS.DataConsumer.MessageNotification>(o);
        }
        public MessageNotificationT UnPack()
        {
            var _o = new MessageNotificationT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(MessageNotificationT _o)
        {
            _o.Ppid = this.Ppid;
            _o.Data = new List<byte>();
            for(var _j = 0; _j < this.DataLength; ++_j)
            { _o.Data.Add(this.Data(_j)); }
        }
        public static Offset<FBS.DataConsumer.MessageNotification> Pack(FlatBufferBuilder builder, MessageNotificationT _o)
        {
            if(_o == null)
                return default(Offset<FBS.DataConsumer.MessageNotification>);
            var _data = default(VectorOffset);
            if(_o.Data != null)
            {
                var __data = _o.Data.ToArray();
                _data = CreateDataVector(builder, __data);
            }
            return CreateMessageNotification(
              builder,
              _o.Ppid,
              _data);
        }
    }

    public class MessageNotificationT
    {
        public uint Ppid { get; set; }

        /// <summary>
        /// TODO: Do not use `List<byte>`
        /// </summary>
        public List<byte> Data { get; set; }
    }
}
