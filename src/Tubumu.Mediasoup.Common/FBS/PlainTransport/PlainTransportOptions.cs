// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FBS.PlainTransport
{
    using Google.FlatBuffers;
    using System.Text.Json.Serialization;

    public struct PlainTransportOptions : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static PlainTransportOptions GetRootAsPlainTransportOptions(ByteBuffer _bb) { return GetRootAsPlainTransportOptions(_bb, new PlainTransportOptions()); }
        public static PlainTransportOptions GetRootAsPlainTransportOptions(ByteBuffer _bb, PlainTransportOptions obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public PlainTransportOptions __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.Transport.Options? Base { get { int o = __p.__offset(4); return o != 0 ? (FBS.Transport.Options?)(new FBS.Transport.Options()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public FBS.Transport.ListenInfo? ListenInfo { get { int o = __p.__offset(6); return o != 0 ? (FBS.Transport.ListenInfo?)(new FBS.Transport.ListenInfo()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public FBS.Transport.ListenInfo? RtcpListenInfo { get { int o = __p.__offset(8); return o != 0 ? (FBS.Transport.ListenInfo?)(new FBS.Transport.ListenInfo()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public bool RtcpMux { get { int o = __p.__offset(10); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }
        public bool Comedia { get { int o = __p.__offset(12); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }
        public bool EnableSrtp { get { int o = __p.__offset(14); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }
        public FBS.SrtpParameters.SrtpCryptoSuite? SrtpCryptoSuite { get { int o = __p.__offset(16); return o != 0 ? (FBS.SrtpParameters.SrtpCryptoSuite)__p.bb.Get(o + __p.bb_pos) : (FBS.SrtpParameters.SrtpCryptoSuite?)null; } }

        public static Offset<FBS.PlainTransport.PlainTransportOptions> CreatePlainTransportOptions(FlatBufferBuilder builder,
            Offset<FBS.Transport.Options> @baseOffset = default(Offset<FBS.Transport.Options>),
            Offset<FBS.Transport.ListenInfo> listen_infoOffset = default(Offset<FBS.Transport.ListenInfo>),
            Offset<FBS.Transport.ListenInfo> rtcp_listen_infoOffset = default(Offset<FBS.Transport.ListenInfo>),
            bool rtcp_mux = false,
            bool comedia = false,
            bool enable_srtp = false,
            FBS.SrtpParameters.SrtpCryptoSuite? srtp_crypto_suite = null)
        {
            builder.StartTable(7);
            PlainTransportOptions.AddRtcpListenInfo(builder, rtcp_listen_infoOffset);
            PlainTransportOptions.AddListenInfo(builder, listen_infoOffset);
            PlainTransportOptions.AddBase(builder, @baseOffset);
            PlainTransportOptions.AddSrtpCryptoSuite(builder, srtp_crypto_suite);
            PlainTransportOptions.AddEnableSrtp(builder, enable_srtp);
            PlainTransportOptions.AddComedia(builder, comedia);
            PlainTransportOptions.AddRtcpMux(builder, rtcp_mux);
            return PlainTransportOptions.EndPlainTransportOptions(builder);
        }

        public static void StartPlainTransportOptions(FlatBufferBuilder builder) { builder.StartTable(7); }
        public static void AddBase(FlatBufferBuilder builder, Offset<FBS.Transport.Options> baseOffset) { builder.AddOffset(0, baseOffset.Value, 0); }
        public static void AddListenInfo(FlatBufferBuilder builder, Offset<FBS.Transport.ListenInfo> listenInfoOffset) { builder.AddOffset(1, listenInfoOffset.Value, 0); }
        public static void AddRtcpListenInfo(FlatBufferBuilder builder, Offset<FBS.Transport.ListenInfo> rtcpListenInfoOffset) { builder.AddOffset(2, rtcpListenInfoOffset.Value, 0); }
        public static void AddRtcpMux(FlatBufferBuilder builder, bool rtcpMux) { builder.AddBool(3, rtcpMux, false); }
        public static void AddComedia(FlatBufferBuilder builder, bool comedia) { builder.AddBool(4, comedia, false); }
        public static void AddEnableSrtp(FlatBufferBuilder builder, bool enableSrtp) { builder.AddBool(5, enableSrtp, false); }
        public static void AddSrtpCryptoSuite(FlatBufferBuilder builder, FBS.SrtpParameters.SrtpCryptoSuite? srtpCryptoSuite) { builder.AddByte(6, (byte?)srtpCryptoSuite); }
        public static Offset<FBS.PlainTransport.PlainTransportOptions> EndPlainTransportOptions(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // base
            builder.Required(o, 6);  // listen_info
            return new Offset<FBS.PlainTransport.PlainTransportOptions>(o);
        }
        public PlainTransportOptionsT UnPack()
        {
            var _o = new PlainTransportOptionsT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(PlainTransportOptionsT _o)
        {
            _o.Base = this.Base.HasValue ? this.Base.Value.UnPack() : null;
            _o.ListenInfo = this.ListenInfo.HasValue ? this.ListenInfo.Value.UnPack() : null;
            _o.RtcpListenInfo = this.RtcpListenInfo.HasValue ? this.RtcpListenInfo.Value.UnPack() : null;
            _o.RtcpMux = this.RtcpMux;
            _o.Comedia = this.Comedia;
            _o.EnableSrtp = this.EnableSrtp;
            _o.SrtpCryptoSuite = this.SrtpCryptoSuite;
        }
        public static Offset<FBS.PlainTransport.PlainTransportOptions> Pack(FlatBufferBuilder builder, PlainTransportOptionsT _o)
        {
            if(_o == null)
                return default(Offset<FBS.PlainTransport.PlainTransportOptions>);
            var _base = _o.Base == null ? default(Offset<FBS.Transport.Options>) : FBS.Transport.Options.Pack(builder, _o.Base);
            var _listen_info = _o.ListenInfo == null ? default(Offset<FBS.Transport.ListenInfo>) : FBS.Transport.ListenInfo.Pack(builder, _o.ListenInfo);
            var _rtcp_listen_info = _o.RtcpListenInfo == null ? default(Offset<FBS.Transport.ListenInfo>) : FBS.Transport.ListenInfo.Pack(builder, _o.RtcpListenInfo);
            return CreatePlainTransportOptions(
              builder,
              _base,
              _listen_info,
              _rtcp_listen_info,
              _o.RtcpMux,
              _o.Comedia,
              _o.EnableSrtp,
              _o.SrtpCryptoSuite);
        }
    }

    public class PlainTransportOptionsT
    {
        [JsonPropertyName("base")]
        public FBS.Transport.OptionsT Base { get; set; }
        [JsonPropertyName("listen_info")]
        public FBS.Transport.ListenInfoT ListenInfo { get; set; }
        [JsonPropertyName("rtcp_listen_info")]
        public FBS.Transport.ListenInfoT RtcpListenInfo { get; set; }
        [JsonPropertyName("rtcp_mux")]
        public bool RtcpMux { get; set; }
        [JsonPropertyName("comedia")]
        public bool Comedia { get; set; }
        [JsonPropertyName("enable_srtp")]
        public bool EnableSrtp { get; set; }
        [JsonPropertyName("srtp_crypto_suite")]
        public FBS.SrtpParameters.SrtpCryptoSuite? SrtpCryptoSuite { get; set; }

        public PlainTransportOptionsT()
        {
            this.Base = null;
            this.ListenInfo = null;
            this.RtcpListenInfo = null;
            this.RtcpMux = false;
            this.Comedia = false;
            this.EnableSrtp = false;
            this.SrtpCryptoSuite = null;
        }
    }
}
