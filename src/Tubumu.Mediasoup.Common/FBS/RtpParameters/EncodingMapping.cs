// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.RtpParameters
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct EncodingMapping : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static EncodingMapping GetRootAsEncodingMapping(ByteBuffer _bb) { return GetRootAsEncodingMapping(_bb, new EncodingMapping()); }
        public static EncodingMapping GetRootAsEncodingMapping(ByteBuffer _bb, EncodingMapping obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public EncodingMapping __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string Rid { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetRidBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetRidBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetRidArray() { return __p.__vector_as_array<byte>(4); }
        public uint? Ssrc { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint?)null; } }
        public string ScalabilityMode { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetScalabilityModeBytes() { return __p.__vector_as_span<byte>(8, 1); }
#else
        public ArraySegment<byte>? GetScalabilityModeBytes() { return __p.__vector_as_arraysegment(8); }
#endif
        public byte[] GetScalabilityModeArray() { return __p.__vector_as_array<byte>(8); }
        public uint MappedSsrc { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }

        public static Offset<FBS.RtpParameters.EncodingMapping> CreateEncodingMapping(FlatBufferBuilder builder,
            StringOffset ridOffset = default(StringOffset),
            uint? ssrc = null,
            StringOffset scalability_modeOffset = default(StringOffset),
            uint mapped_ssrc = 0)
        {
            builder.StartTable(4);
            EncodingMapping.AddMappedSsrc(builder, mapped_ssrc);
            EncodingMapping.AddScalabilityMode(builder, scalability_modeOffset);
            EncodingMapping.AddSsrc(builder, ssrc);
            EncodingMapping.AddRid(builder, ridOffset);
            return EncodingMapping.EndEncodingMapping(builder);
        }

        public static void StartEncodingMapping(FlatBufferBuilder builder) { builder.StartTable(4); }
        public static void AddRid(FlatBufferBuilder builder, StringOffset ridOffset) { builder.AddOffset(0, ridOffset.Value, 0); }
        public static void AddSsrc(FlatBufferBuilder builder, uint? ssrc) { builder.AddUint(1, ssrc); }
        public static void AddScalabilityMode(FlatBufferBuilder builder, StringOffset scalabilityModeOffset) { builder.AddOffset(2, scalabilityModeOffset.Value, 0); }
        public static void AddMappedSsrc(FlatBufferBuilder builder, uint mappedSsrc) { builder.AddUint(3, mappedSsrc, 0); }
        public static Offset<FBS.RtpParameters.EncodingMapping> EndEncodingMapping(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.RtpParameters.EncodingMapping>(o);
        }
        public EncodingMappingT UnPack()
        {
            var _o = new EncodingMappingT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(EncodingMappingT _o)
        {
            _o.Rid = this.Rid;
            _o.Ssrc = this.Ssrc;
            _o.ScalabilityMode = this.ScalabilityMode;
            _o.MappedSsrc = this.MappedSsrc;
        }
        public static Offset<FBS.RtpParameters.EncodingMapping> Pack(FlatBufferBuilder builder, EncodingMappingT _o)
        {
            if(_o == null)
                return default(Offset<FBS.RtpParameters.EncodingMapping>);
            var _rid = _o.Rid == null ? default(StringOffset) : builder.CreateString(_o.Rid);
            var _scalability_mode = _o.ScalabilityMode == null ? default(StringOffset) : builder.CreateString(_o.ScalabilityMode);
            return CreateEncodingMapping(
              builder,
              _rid,
              _o.Ssrc,
              _scalability_mode,
              _o.MappedSsrc);
        }
    }

    public class EncodingMappingT
    {
        public string Rid { get; set; }

        public uint? Ssrc { get; set; }

        public string ScalabilityMode { get; set; }

        public uint MappedSsrc { get; set; }

        public EncodingMappingT()
        {
            this.Rid = null;
            this.Ssrc = null;
            this.ScalabilityMode = null;
            this.MappedSsrc = 0;
        }
    }


    static public class EncodingMappingVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyString(tablePos, 4 /*Rid*/, false)
              && verifier.VerifyField(tablePos, 6 /*Ssrc*/, 4 /*uint*/, 4, false)
              && verifier.VerifyString(tablePos, 8 /*ScalabilityMode*/, false)
              && verifier.VerifyField(tablePos, 10 /*MappedSsrc*/, 4 /*uint*/, 4, false)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
