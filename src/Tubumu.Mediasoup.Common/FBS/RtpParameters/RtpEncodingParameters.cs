// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.RtpParameters
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct RtpEncodingParameters : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static RtpEncodingParameters GetRootAsRtpEncodingParameters(ByteBuffer _bb) { return GetRootAsRtpEncodingParameters(_bb, new RtpEncodingParameters()); }
        public static RtpEncodingParameters GetRootAsRtpEncodingParameters(ByteBuffer _bb, RtpEncodingParameters obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public RtpEncodingParameters __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public uint? Ssrc { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint?)null; } }
        public string Rid { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetRidBytes() { return __p.__vector_as_span<byte>(6, 1); }
#else
        public ArraySegment<byte>? GetRidBytes() { return __p.__vector_as_arraysegment(6); }
#endif
        public byte[] GetRidArray() { return __p.__vector_as_array<byte>(6); }
        public byte? CodecPayloadType { get { int o = __p.__offset(8); return o != 0 ? __p.bb.Get(o + __p.bb_pos) : (byte?)null; } }
        public FBS.RtpParameters.Rtx? Rtx { get { int o = __p.__offset(10); return o != 0 ? (FBS.RtpParameters.Rtx?)(new FBS.RtpParameters.Rtx()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public bool Dtx { get { int o = __p.__offset(12); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }
        public string ScalabilityMode { get { int o = __p.__offset(14); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetScalabilityModeBytes() { return __p.__vector_as_span<byte>(14, 1); }
#else
        public ArraySegment<byte>? GetScalabilityModeBytes() { return __p.__vector_as_arraysegment(14); }
#endif
        public byte[] GetScalabilityModeArray() { return __p.__vector_as_array<byte>(14); }
        public uint? MaxBitrate { get { int o = __p.__offset(16); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint?)null; } }

        public static Offset<FBS.RtpParameters.RtpEncodingParameters> CreateRtpEncodingParameters(FlatBufferBuilder builder,
            uint? ssrc = null,
            StringOffset ridOffset = default(StringOffset),
            byte? codec_payload_type = null,
            Offset<FBS.RtpParameters.Rtx> rtxOffset = default(Offset<FBS.RtpParameters.Rtx>),
            bool dtx = false,
            StringOffset scalability_modeOffset = default(StringOffset),
            uint? max_bitrate = null)
        {
            builder.StartTable(7);
            RtpEncodingParameters.AddMaxBitrate(builder, max_bitrate);
            RtpEncodingParameters.AddScalabilityMode(builder, scalability_modeOffset);
            RtpEncodingParameters.AddRtx(builder, rtxOffset);
            RtpEncodingParameters.AddRid(builder, ridOffset);
            RtpEncodingParameters.AddSsrc(builder, ssrc);
            RtpEncodingParameters.AddDtx(builder, dtx);
            RtpEncodingParameters.AddCodecPayloadType(builder, codec_payload_type);
            return RtpEncodingParameters.EndRtpEncodingParameters(builder);
        }

        public static void StartRtpEncodingParameters(FlatBufferBuilder builder) { builder.StartTable(7); }
        public static void AddSsrc(FlatBufferBuilder builder, uint? ssrc) { builder.AddUint(0, ssrc); }
        public static void AddRid(FlatBufferBuilder builder, StringOffset ridOffset) { builder.AddOffset(1, ridOffset.Value, 0); }
        public static void AddCodecPayloadType(FlatBufferBuilder builder, byte? codecPayloadType) { builder.AddByte(2, codecPayloadType); }
        public static void AddRtx(FlatBufferBuilder builder, Offset<FBS.RtpParameters.Rtx> rtxOffset) { builder.AddOffset(3, rtxOffset.Value, 0); }
        public static void AddDtx(FlatBufferBuilder builder, bool dtx) { builder.AddBool(4, dtx, false); }
        public static void AddScalabilityMode(FlatBufferBuilder builder, StringOffset scalabilityModeOffset) { builder.AddOffset(5, scalabilityModeOffset.Value, 0); }
        public static void AddMaxBitrate(FlatBufferBuilder builder, uint? maxBitrate) { builder.AddUint(6, maxBitrate); }
        public static Offset<FBS.RtpParameters.RtpEncodingParameters> EndRtpEncodingParameters(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.RtpParameters.RtpEncodingParameters>(o);
        }
        public RtpEncodingParametersT UnPack()
        {
            var _o = new RtpEncodingParametersT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(RtpEncodingParametersT _o)
        {
            _o.Ssrc = this.Ssrc;
            _o.Rid = this.Rid;
            _o.CodecPayloadType = this.CodecPayloadType;
            _o.Rtx = this.Rtx.HasValue ? this.Rtx.Value.UnPack() : null;
            _o.Dtx = this.Dtx;
            _o.ScalabilityMode = this.ScalabilityMode;
            _o.MaxBitrate = this.MaxBitrate;
        }
        public static Offset<FBS.RtpParameters.RtpEncodingParameters> Pack(FlatBufferBuilder builder, RtpEncodingParametersT _o)
        {
            if(_o == null)
                return default(Offset<FBS.RtpParameters.RtpEncodingParameters>);
            var _rid = _o.Rid == null ? default(StringOffset) : builder.CreateString(_o.Rid);
            var _rtx = _o.Rtx == null ? default(Offset<FBS.RtpParameters.Rtx>) : FBS.RtpParameters.Rtx.Pack(builder, _o.Rtx);
            var _scalability_mode = _o.ScalabilityMode == null ? default(StringOffset) : builder.CreateString(_o.ScalabilityMode);
            return CreateRtpEncodingParameters(
              builder,
              _o.Ssrc,
              _rid,
              _o.CodecPayloadType,
              _rtx,
              _o.Dtx,
              _scalability_mode,
              _o.MaxBitrate);
        }
    }

    public class RtpEncodingParametersT
    {
        public uint? Ssrc { get; set; }

        public string Rid { get; set; }

        public byte? CodecPayloadType { get; set; }

        public FBS.RtpParameters.RtxT Rtx { get; set; }

        public bool Dtx { get; set; }

        public string ScalabilityMode { get; set; }

        public uint? MaxBitrate { get; set; }

        public RtpEncodingParametersT()
        {
            this.Ssrc = null;
            this.Rid = null;
            this.CodecPayloadType = null;
            this.Rtx = null;
            this.Dtx = false;
            this.ScalabilityMode = null;
            this.MaxBitrate = null;
        }
    }


    static public class RtpEncodingParametersVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyField(tablePos, 4 /*Ssrc*/, 4 /*uint*/, 4, false)
              && verifier.VerifyString(tablePos, 6 /*Rid*/, false)
              && verifier.VerifyField(tablePos, 8 /*CodecPayloadType*/, 1 /*byte*/, 1, false)
              && verifier.VerifyTable(tablePos, 10 /*Rtx*/, FBS.RtpParameters.RtxVerify.Verify, false)
              && verifier.VerifyField(tablePos, 12 /*Dtx*/, 1 /*bool*/, 1, false)
              && verifier.VerifyString(tablePos, 14 /*ScalabilityMode*/, false)
              && verifier.VerifyField(tablePos, 16 /*MaxBitrate*/, 4 /*uint*/, 4, false)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
