// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.WebRtcTransport
{
    public struct IceStateChangeNotification : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static IceStateChangeNotification GetRootAsIceStateChangeNotification(ByteBuffer _bb) { return GetRootAsIceStateChangeNotification(_bb, new IceStateChangeNotification()); }
        public static IceStateChangeNotification GetRootAsIceStateChangeNotification(ByteBuffer _bb, IceStateChangeNotification obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public IceStateChangeNotification __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.WebRtcTransport.IceState IceState { get { int o = __p.__offset(4); return o != 0 ? (FBS.WebRtcTransport.IceState)__p.bb.Get(o + __p.bb_pos) : FBS.WebRtcTransport.IceState.NEW; } }

        public static Offset<FBS.WebRtcTransport.IceStateChangeNotification> CreateIceStateChangeNotification(FlatBufferBuilder builder,
            FBS.WebRtcTransport.IceState ice_state = FBS.WebRtcTransport.IceState.NEW)
        {
            builder.StartTable(1);
            IceStateChangeNotification.AddIceState(builder, ice_state);
            return IceStateChangeNotification.EndIceStateChangeNotification(builder);
        }

        public static void StartIceStateChangeNotification(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddIceState(FlatBufferBuilder builder, FBS.WebRtcTransport.IceState iceState) { builder.AddByte(0, (byte)iceState, 0); }
        public static Offset<FBS.WebRtcTransport.IceStateChangeNotification> EndIceStateChangeNotification(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.WebRtcTransport.IceStateChangeNotification>(o);
        }
        public IceStateChangeNotificationT UnPack()
        {
            var _o = new IceStateChangeNotificationT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(IceStateChangeNotificationT _o)
        {
            _o.IceState = this.IceState;
        }
        public static Offset<FBS.WebRtcTransport.IceStateChangeNotification> Pack(FlatBufferBuilder builder, IceStateChangeNotificationT _o)
        {
            if(_o == null)
                return default(Offset<FBS.WebRtcTransport.IceStateChangeNotification>);
            return CreateIceStateChangeNotification(
              builder,
              _o.IceState);
        }
    }

    public class IceStateChangeNotificationT
    {
        [JsonPropertyName("ice_state")]
        public FBS.WebRtcTransport.IceState IceState { get; set; }

        public IceStateChangeNotificationT()
        {
            this.IceState = FBS.WebRtcTransport.IceState.NEW;
        }
    }
}