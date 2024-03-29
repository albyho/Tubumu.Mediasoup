// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.ActiveSpeakerObserver
{
    public struct DominantSpeakerNotification : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static DominantSpeakerNotification GetRootAsDominantSpeakerNotification(ByteBuffer _bb) { return GetRootAsDominantSpeakerNotification(_bb, new DominantSpeakerNotification()); }
        public static DominantSpeakerNotification GetRootAsDominantSpeakerNotification(ByteBuffer _bb, DominantSpeakerNotification obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public DominantSpeakerNotification __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string ProducerId { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetProducerIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetProducerIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetProducerIdArray() { return __p.__vector_as_array<byte>(4); }

        public static Offset<FBS.ActiveSpeakerObserver.DominantSpeakerNotification> CreateDominantSpeakerNotification(FlatBufferBuilder builder,
            StringOffset producer_idOffset = default(StringOffset))
        {
            builder.StartTable(1);
            DominantSpeakerNotification.AddProducerId(builder, producer_idOffset);
            return DominantSpeakerNotification.EndDominantSpeakerNotification(builder);
        }

        public static void StartDominantSpeakerNotification(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddProducerId(FlatBufferBuilder builder, StringOffset producerIdOffset) { builder.AddOffset(0, producerIdOffset.Value, 0); }
        public static Offset<FBS.ActiveSpeakerObserver.DominantSpeakerNotification> EndDominantSpeakerNotification(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // producer_id
            return new Offset<FBS.ActiveSpeakerObserver.DominantSpeakerNotification>(o);
        }
        public DominantSpeakerNotificationT UnPack()
        {
            var _o = new DominantSpeakerNotificationT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(DominantSpeakerNotificationT _o)
        {
            _o.ProducerId = this.ProducerId;
        }
        public static Offset<FBS.ActiveSpeakerObserver.DominantSpeakerNotification> Pack(FlatBufferBuilder builder, DominantSpeakerNotificationT _o)
        {
            if(_o == null)
                return default(Offset<FBS.ActiveSpeakerObserver.DominantSpeakerNotification>);
            var _producer_id = _o.ProducerId == null ? default(StringOffset) : builder.CreateString(_o.ProducerId);
            return CreateDominantSpeakerNotification(
              builder,
              _producer_id);
        }
    }
}
