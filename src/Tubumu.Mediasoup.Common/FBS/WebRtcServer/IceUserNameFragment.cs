// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.WebRtcServer
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct IceUserNameFragment : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static IceUserNameFragment GetRootAsIceUserNameFragment(ByteBuffer _bb) { return GetRootAsIceUserNameFragment(_bb, new IceUserNameFragment()); }
        public static IceUserNameFragment GetRootAsIceUserNameFragment(ByteBuffer _bb, IceUserNameFragment obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public IceUserNameFragment __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string LocalIceUsernameFragment { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetLocalIceUsernameFragmentBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetLocalIceUsernameFragmentBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetLocalIceUsernameFragmentArray() { return __p.__vector_as_array<byte>(4); }
        public string WebRtcTransportId { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetWebRtcTransportIdBytes() { return __p.__vector_as_span<byte>(6, 1); }
#else
        public ArraySegment<byte>? GetWebRtcTransportIdBytes() { return __p.__vector_as_arraysegment(6); }
#endif
        public byte[] GetWebRtcTransportIdArray() { return __p.__vector_as_array<byte>(6); }

        public static Offset<FBS.WebRtcServer.IceUserNameFragment> CreateIceUserNameFragment(FlatBufferBuilder builder,
            StringOffset local_ice_username_fragmentOffset = default(StringOffset),
            StringOffset web_rtc_transport_idOffset = default(StringOffset))
        {
            builder.StartTable(2);
            IceUserNameFragment.AddWebRtcTransportId(builder, web_rtc_transport_idOffset);
            IceUserNameFragment.AddLocalIceUsernameFragment(builder, local_ice_username_fragmentOffset);
            return IceUserNameFragment.EndIceUserNameFragment(builder);
        }

        public static void StartIceUserNameFragment(FlatBufferBuilder builder) { builder.StartTable(2); }
        public static void AddLocalIceUsernameFragment(FlatBufferBuilder builder, StringOffset localIceUsernameFragmentOffset) { builder.AddOffset(0, localIceUsernameFragmentOffset.Value, 0); }
        public static void AddWebRtcTransportId(FlatBufferBuilder builder, StringOffset webRtcTransportIdOffset) { builder.AddOffset(1, webRtcTransportIdOffset.Value, 0); }
        public static Offset<FBS.WebRtcServer.IceUserNameFragment> EndIceUserNameFragment(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // local_ice_username_fragment
            builder.Required(o, 6);  // web_rtc_transport_id
            return new Offset<FBS.WebRtcServer.IceUserNameFragment>(o);
        }
        public IceUserNameFragmentT UnPack()
        {
            var _o = new IceUserNameFragmentT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(IceUserNameFragmentT _o)
        {
            _o.LocalIceUsernameFragment = this.LocalIceUsernameFragment;
            _o.WebRtcTransportId = this.WebRtcTransportId;
        }
        public static Offset<FBS.WebRtcServer.IceUserNameFragment> Pack(FlatBufferBuilder builder, IceUserNameFragmentT _o)
        {
            if(_o == null)
                return default(Offset<FBS.WebRtcServer.IceUserNameFragment>);
            var _local_ice_username_fragment = _o.LocalIceUsernameFragment == null ? default(StringOffset) : builder.CreateString(_o.LocalIceUsernameFragment);
            var _web_rtc_transport_id = _o.WebRtcTransportId == null ? default(StringOffset) : builder.CreateString(_o.WebRtcTransportId);
            return CreateIceUserNameFragment(
              builder,
              _local_ice_username_fragment,
              _web_rtc_transport_id);
        }
    }

    public class IceUserNameFragmentT
    {
        public string LocalIceUsernameFragment { get; set; }

        public string WebRtcTransportId { get; set; }

        public IceUserNameFragmentT()
        {
            this.LocalIceUsernameFragment = null;
            this.WebRtcTransportId = null;
        }
    }


    static public class IceUserNameFragmentVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyString(tablePos, 4 /*LocalIceUsernameFragment*/, true)
              && verifier.VerifyString(tablePos, 6 /*WebRtcTransportId*/, true)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
