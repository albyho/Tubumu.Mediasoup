// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.Transport
{
    public struct SocketFlags : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static SocketFlags GetRootAsSocketFlags(ByteBuffer _bb) { return GetRootAsSocketFlags(_bb, new SocketFlags()); }
        public static SocketFlags GetRootAsSocketFlags(ByteBuffer _bb, SocketFlags obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public SocketFlags __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public bool Ipv6Only { get { int o = __p.__offset(4); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }
        public bool UdpReusePort { get { int o = __p.__offset(6); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }

        public static Offset<FBS.Transport.SocketFlags> CreateSocketFlags(FlatBufferBuilder builder,
            bool ipv6_only = false,
            bool udp_reuse_port = false)
        {
            builder.StartTable(2);
            SocketFlags.AddUdpReusePort(builder, udp_reuse_port);
            SocketFlags.AddIpv6Only(builder, ipv6_only);
            return SocketFlags.EndSocketFlags(builder);
        }

        public static void StartSocketFlags(FlatBufferBuilder builder) { builder.StartTable(2); }
        public static void AddIpv6Only(FlatBufferBuilder builder, bool ipv6Only) { builder.AddBool(0, ipv6Only, false); }
        public static void AddUdpReusePort(FlatBufferBuilder builder, bool udpReusePort) { builder.AddBool(1, udpReusePort, false); }
        public static Offset<FBS.Transport.SocketFlags> EndSocketFlags(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.Transport.SocketFlags>(o);
        }
        public SocketFlagsT UnPack()
        {
            var _o = new SocketFlagsT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(SocketFlagsT _o)
        {
            _o.Ipv6Only = this.Ipv6Only;
            _o.UdpReusePort = this.UdpReusePort;
        }
        public static Offset<FBS.Transport.SocketFlags> Pack(FlatBufferBuilder builder, SocketFlagsT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Transport.SocketFlags>);
            return CreateSocketFlags(
              builder,
              _o.Ipv6Only,
              _o.UdpReusePort);
        }
    }
}
