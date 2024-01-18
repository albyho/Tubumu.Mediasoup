// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.WebRtcServer
{
    public struct IpPort : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static IpPort GetRootAsIpPort(ByteBuffer _bb) { return GetRootAsIpPort(_bb, new IpPort()); }
        public static IpPort GetRootAsIpPort(ByteBuffer _bb, IpPort obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public IpPort __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string Ip { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetIpBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetIpBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetIpArray() { return __p.__vector_as_array<byte>(4); }
        public ushort Port { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUshort(o + __p.bb_pos) : (ushort)0; } }

        public static Offset<FBS.WebRtcServer.IpPort> CreateIpPort(FlatBufferBuilder builder,
            StringOffset ipOffset = default(StringOffset),
            ushort port = 0)
        {
            builder.StartTable(2);
            IpPort.AddIp(builder, ipOffset);
            IpPort.AddPort(builder, port);
            return IpPort.EndIpPort(builder);
        }

        public static void StartIpPort(FlatBufferBuilder builder) { builder.StartTable(2); }
        public static void AddIp(FlatBufferBuilder builder, StringOffset ipOffset) { builder.AddOffset(0, ipOffset.Value, 0); }
        public static void AddPort(FlatBufferBuilder builder, ushort port) { builder.AddUshort(1, port, 0); }
        public static Offset<FBS.WebRtcServer.IpPort> EndIpPort(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // ip
            return new Offset<FBS.WebRtcServer.IpPort>(o);
        }
        public IpPortT UnPack()
        {
            var _o = new IpPortT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(IpPortT _o)
        {
            _o.Ip = this.Ip;
            _o.Port = this.Port;
        }
        public static Offset<FBS.WebRtcServer.IpPort> Pack(FlatBufferBuilder builder, IpPortT _o)
        {
            if(_o == null)
                return default(Offset<FBS.WebRtcServer.IpPort>);
            var _ip = _o.Ip == null ? default(StringOffset) : builder.CreateString(_o.Ip);
            return CreateIpPort(
              builder,
              _ip,
              _o.Port);
        }
    }

    public class IpPortT
    {
        [JsonPropertyName("ip")]
        public string Ip { get; set; }
        [JsonPropertyName("port")]
        public ushort Port { get; set; }

        public IpPortT()
        {
            this.Ip = null;
            this.Port = 0;
        }
    }
}