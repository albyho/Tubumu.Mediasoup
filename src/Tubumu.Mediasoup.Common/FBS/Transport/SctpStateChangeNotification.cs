// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.Transport
{
    public struct SctpStateChangeNotification : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static SctpStateChangeNotification GetRootAsSctpStateChangeNotification(ByteBuffer _bb) { return GetRootAsSctpStateChangeNotification(_bb, new SctpStateChangeNotification()); }
        public static SctpStateChangeNotification GetRootAsSctpStateChangeNotification(ByteBuffer _bb, SctpStateChangeNotification obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public SctpStateChangeNotification __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.SctpAssociation.SctpState SctpState { get { int o = __p.__offset(4); return o != 0 ? (FBS.SctpAssociation.SctpState)__p.bb.Get(o + __p.bb_pos) : FBS.SctpAssociation.SctpState.NEW; } }

        public static Offset<FBS.Transport.SctpStateChangeNotification> CreateSctpStateChangeNotification(FlatBufferBuilder builder,
            FBS.SctpAssociation.SctpState sctp_state = FBS.SctpAssociation.SctpState.NEW)
        {
            builder.StartTable(1);
            SctpStateChangeNotification.AddSctpState(builder, sctp_state);
            return SctpStateChangeNotification.EndSctpStateChangeNotification(builder);
        }

        public static void StartSctpStateChangeNotification(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddSctpState(FlatBufferBuilder builder, FBS.SctpAssociation.SctpState sctpState) { builder.AddByte(0, (byte)sctpState, 0); }
        public static Offset<FBS.Transport.SctpStateChangeNotification> EndSctpStateChangeNotification(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.Transport.SctpStateChangeNotification>(o);
        }
        public SctpStateChangeNotificationT UnPack()
        {
            var _o = new SctpStateChangeNotificationT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(SctpStateChangeNotificationT _o)
        {
            _o.SctpState = this.SctpState;
        }
        public static Offset<FBS.Transport.SctpStateChangeNotification> Pack(FlatBufferBuilder builder, SctpStateChangeNotificationT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Transport.SctpStateChangeNotification>);
            return CreateSctpStateChangeNotification(
              builder,
              _o.SctpState);
        }
    }

    public class SctpStateChangeNotificationT
    {
        [JsonPropertyName("sctp_state")]
        public FBS.SctpAssociation.SctpState SctpState { get; set; }

        public SctpStateChangeNotificationT()
        {
            this.SctpState = FBS.SctpAssociation.SctpState.NEW;
        }
    }
}