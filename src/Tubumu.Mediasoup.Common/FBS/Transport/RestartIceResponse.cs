// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.Transport
{
    public struct RestartIceResponse : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static RestartIceResponse GetRootAsRestartIceResponse(ByteBuffer _bb) { return GetRootAsRestartIceResponse(_bb, new RestartIceResponse()); }
        public static RestartIceResponse GetRootAsRestartIceResponse(ByteBuffer _bb, RestartIceResponse obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public RestartIceResponse __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string UsernameFragment { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetUsernameFragmentBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetUsernameFragmentBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetUsernameFragmentArray() { return __p.__vector_as_array<byte>(4); }
        public string Password { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetPasswordBytes() { return __p.__vector_as_span<byte>(6, 1); }
#else
        public ArraySegment<byte>? GetPasswordBytes() { return __p.__vector_as_arraysegment(6); }
#endif
        public byte[] GetPasswordArray() { return __p.__vector_as_array<byte>(6); }
        public bool IceLite { get { int o = __p.__offset(8); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }

        public static Offset<FBS.Transport.RestartIceResponse> CreateRestartIceResponse(FlatBufferBuilder builder,
            StringOffset username_fragmentOffset = default(StringOffset),
            StringOffset passwordOffset = default(StringOffset),
            bool ice_lite = false)
        {
            builder.StartTable(3);
            RestartIceResponse.AddPassword(builder, passwordOffset);
            RestartIceResponse.AddUsernameFragment(builder, username_fragmentOffset);
            RestartIceResponse.AddIceLite(builder, ice_lite);
            return RestartIceResponse.EndRestartIceResponse(builder);
        }

        public static void StartRestartIceResponse(FlatBufferBuilder builder) { builder.StartTable(3); }
        public static void AddUsernameFragment(FlatBufferBuilder builder, StringOffset usernameFragmentOffset) { builder.AddOffset(0, usernameFragmentOffset.Value, 0); }
        public static void AddPassword(FlatBufferBuilder builder, StringOffset passwordOffset) { builder.AddOffset(1, passwordOffset.Value, 0); }
        public static void AddIceLite(FlatBufferBuilder builder, bool iceLite) { builder.AddBool(2, iceLite, false); }
        public static Offset<FBS.Transport.RestartIceResponse> EndRestartIceResponse(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // username_fragment
            builder.Required(o, 6);  // password
            return new Offset<FBS.Transport.RestartIceResponse>(o);
        }
        public RestartIceResponseT UnPack()
        {
            var _o = new RestartIceResponseT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(RestartIceResponseT _o)
        {
            _o.UsernameFragment = this.UsernameFragment;
            _o.Password = this.Password;
            _o.IceLite = this.IceLite;
        }
        public static Offset<FBS.Transport.RestartIceResponse> Pack(FlatBufferBuilder builder, RestartIceResponseT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Transport.RestartIceResponse>);
            var _username_fragment = _o.UsernameFragment == null ? default(StringOffset) : builder.CreateString(_o.UsernameFragment);
            var _password = _o.Password == null ? default(StringOffset) : builder.CreateString(_o.Password);
            return CreateRestartIceResponse(
              builder,
              _username_fragment,
              _password,
              _o.IceLite);
        }
    }

    public class RestartIceResponseT
    {
        [JsonPropertyName("username_fragment")]
        public string UsernameFragment { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [JsonPropertyName("ice_lite")]
        public bool IceLite { get; set; }

        public RestartIceResponseT()
        {
            this.UsernameFragment = null;
            this.Password = null;
            this.IceLite = false;
        }
    }
}
