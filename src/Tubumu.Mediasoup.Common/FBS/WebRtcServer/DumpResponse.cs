// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.WebRtcServer
{
    public struct DumpResponse : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static DumpResponse GetRootAsDumpResponse(ByteBuffer _bb) { return GetRootAsDumpResponse(_bb, new DumpResponse()); }
        public static DumpResponse GetRootAsDumpResponse(ByteBuffer _bb, DumpResponse obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public DumpResponse __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string Id { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
        public ArraySegment<byte>? GetIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
        public byte[] GetIdArray() { return __p.__vector_as_array<byte>(4); }
        public FBS.WebRtcServer.IpPort? UdpSockets(int j) { int o = __p.__offset(6); return o != 0 ? (FBS.WebRtcServer.IpPort?)(new FBS.WebRtcServer.IpPort()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int UdpSocketsLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }
        public FBS.WebRtcServer.IpPort? TcpServers(int j) { int o = __p.__offset(8); return o != 0 ? (FBS.WebRtcServer.IpPort?)(new FBS.WebRtcServer.IpPort()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int TcpServersLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
        public string WebRtcTransportIds(int j) { int o = __p.__offset(10); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : null; }
        public int WebRtcTransportIdsLength { get { int o = __p.__offset(10); return o != 0 ? __p.__vector_len(o) : 0; } }
        public FBS.WebRtcServer.IceUserNameFragment? LocalIceUsernameFragments(int j) { int o = __p.__offset(12); return o != 0 ? (FBS.WebRtcServer.IceUserNameFragment?)(new FBS.WebRtcServer.IceUserNameFragment()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int LocalIceUsernameFragmentsLength { get { int o = __p.__offset(12); return o != 0 ? __p.__vector_len(o) : 0; } }
        public FBS.WebRtcServer.TupleHash? TupleHashes(int j) { int o = __p.__offset(14); return o != 0 ? (FBS.WebRtcServer.TupleHash?)(new FBS.WebRtcServer.TupleHash()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int TupleHashesLength { get { int o = __p.__offset(14); return o != 0 ? __p.__vector_len(o) : 0; } }

        public static Offset<FBS.WebRtcServer.DumpResponse> CreateDumpResponse(FlatBufferBuilder builder,
            StringOffset idOffset = default(StringOffset),
            VectorOffset udp_socketsOffset = default(VectorOffset),
            VectorOffset tcp_serversOffset = default(VectorOffset),
            VectorOffset web_rtc_transport_idsOffset = default(VectorOffset),
            VectorOffset local_ice_username_fragmentsOffset = default(VectorOffset),
            VectorOffset tuple_hashesOffset = default(VectorOffset))
        {
            builder.StartTable(6);
            DumpResponse.AddTupleHashes(builder, tuple_hashesOffset);
            DumpResponse.AddLocalIceUsernameFragments(builder, local_ice_username_fragmentsOffset);
            DumpResponse.AddWebRtcTransportIds(builder, web_rtc_transport_idsOffset);
            DumpResponse.AddTcpServers(builder, tcp_serversOffset);
            DumpResponse.AddUdpSockets(builder, udp_socketsOffset);
            DumpResponse.AddId(builder, idOffset);
            return DumpResponse.EndDumpResponse(builder);
        }

        public static void StartDumpResponse(FlatBufferBuilder builder) { builder.StartTable(6); }
        public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
        public static void AddUdpSockets(FlatBufferBuilder builder, VectorOffset udpSocketsOffset) { builder.AddOffset(1, udpSocketsOffset.Value, 0); }
        public static VectorOffset CreateUdpSocketsVector(FlatBufferBuilder builder, Offset<FBS.WebRtcServer.IpPort>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateUdpSocketsVectorBlock(FlatBufferBuilder builder, Offset<FBS.WebRtcServer.IpPort>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateUdpSocketsVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.WebRtcServer.IpPort>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateUdpSocketsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.WebRtcServer.IpPort>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartUdpSocketsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddTcpServers(FlatBufferBuilder builder, VectorOffset tcpServersOffset) { builder.AddOffset(2, tcpServersOffset.Value, 0); }
        public static VectorOffset CreateTcpServersVector(FlatBufferBuilder builder, Offset<FBS.WebRtcServer.IpPort>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateTcpServersVectorBlock(FlatBufferBuilder builder, Offset<FBS.WebRtcServer.IpPort>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateTcpServersVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.WebRtcServer.IpPort>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateTcpServersVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.WebRtcServer.IpPort>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartTcpServersVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddWebRtcTransportIds(FlatBufferBuilder builder, VectorOffset webRtcTransportIdsOffset) { builder.AddOffset(3, webRtcTransportIdsOffset.Value, 0); }
        public static VectorOffset CreateWebRtcTransportIdsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateWebRtcTransportIdsVectorBlock(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateWebRtcTransportIdsVectorBlock(FlatBufferBuilder builder, ArraySegment<StringOffset> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateWebRtcTransportIdsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<StringOffset>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartWebRtcTransportIdsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddLocalIceUsernameFragments(FlatBufferBuilder builder, VectorOffset localIceUsernameFragmentsOffset) { builder.AddOffset(4, localIceUsernameFragmentsOffset.Value, 0); }
        public static VectorOffset CreateLocalIceUsernameFragmentsVector(FlatBufferBuilder builder, Offset<FBS.WebRtcServer.IceUserNameFragment>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateLocalIceUsernameFragmentsVectorBlock(FlatBufferBuilder builder, Offset<FBS.WebRtcServer.IceUserNameFragment>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateLocalIceUsernameFragmentsVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.WebRtcServer.IceUserNameFragment>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateLocalIceUsernameFragmentsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.WebRtcServer.IceUserNameFragment>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartLocalIceUsernameFragmentsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddTupleHashes(FlatBufferBuilder builder, VectorOffset tupleHashesOffset) { builder.AddOffset(5, tupleHashesOffset.Value, 0); }
        public static VectorOffset CreateTupleHashesVector(FlatBufferBuilder builder, Offset<FBS.WebRtcServer.TupleHash>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateTupleHashesVectorBlock(FlatBufferBuilder builder, Offset<FBS.WebRtcServer.TupleHash>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateTupleHashesVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.WebRtcServer.TupleHash>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateTupleHashesVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.WebRtcServer.TupleHash>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartTupleHashesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static Offset<FBS.WebRtcServer.DumpResponse> EndDumpResponse(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // id
            builder.Required(o, 6);  // udp_sockets
            builder.Required(o, 8);  // tcp_servers
            builder.Required(o, 10);  // web_rtc_transport_ids
            builder.Required(o, 12);  // local_ice_username_fragments
            builder.Required(o, 14);  // tuple_hashes
            return new Offset<FBS.WebRtcServer.DumpResponse>(o);
        }
        public DumpResponseT UnPack()
        {
            var _o = new DumpResponseT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(DumpResponseT _o)
        {
            _o.Id = this.Id;
            _o.UdpSockets = new List<FBS.WebRtcServer.IpPortT>();
            for(var _j = 0; _j < this.UdpSocketsLength; ++_j)
            { _o.UdpSockets.Add(this.UdpSockets(_j).HasValue ? this.UdpSockets(_j).Value.UnPack() : null); }
            _o.TcpServers = new List<FBS.WebRtcServer.IpPortT>();
            for(var _j = 0; _j < this.TcpServersLength; ++_j)
            { _o.TcpServers.Add(this.TcpServers(_j).HasValue ? this.TcpServers(_j).Value.UnPack() : null); }
            _o.WebRtcTransportIds = new List<string>();
            for(var _j = 0; _j < this.WebRtcTransportIdsLength; ++_j)
            { _o.WebRtcTransportIds.Add(this.WebRtcTransportIds(_j)); }
            _o.LocalIceUsernameFragments = new List<FBS.WebRtcServer.IceUserNameFragmentT>();
            for(var _j = 0; _j < this.LocalIceUsernameFragmentsLength; ++_j)
            { _o.LocalIceUsernameFragments.Add(this.LocalIceUsernameFragments(_j).HasValue ? this.LocalIceUsernameFragments(_j).Value.UnPack() : null); }
            _o.TupleHashes = new List<FBS.WebRtcServer.TupleHashT>();
            for(var _j = 0; _j < this.TupleHashesLength; ++_j)
            { _o.TupleHashes.Add(this.TupleHashes(_j).HasValue ? this.TupleHashes(_j).Value.UnPack() : null); }
        }
        public static Offset<FBS.WebRtcServer.DumpResponse> Pack(FlatBufferBuilder builder, DumpResponseT _o)
        {
            if(_o == null)
                return default(Offset<FBS.WebRtcServer.DumpResponse>);
            var _id = _o.Id == null ? default(StringOffset) : builder.CreateString(_o.Id);
            var _udp_sockets = default(VectorOffset);
            if(_o.UdpSockets != null)
            {
                var __udp_sockets = new Offset<FBS.WebRtcServer.IpPort>[_o.UdpSockets.Count];
                for(var _j = 0; _j < __udp_sockets.Length; ++_j)
                { __udp_sockets[_j] = FBS.WebRtcServer.IpPort.Pack(builder, _o.UdpSockets[_j]); }
                _udp_sockets = CreateUdpSocketsVector(builder, __udp_sockets);
            }
            var _tcp_servers = default(VectorOffset);
            if(_o.TcpServers != null)
            {
                var __tcp_servers = new Offset<FBS.WebRtcServer.IpPort>[_o.TcpServers.Count];
                for(var _j = 0; _j < __tcp_servers.Length; ++_j)
                { __tcp_servers[_j] = FBS.WebRtcServer.IpPort.Pack(builder, _o.TcpServers[_j]); }
                _tcp_servers = CreateTcpServersVector(builder, __tcp_servers);
            }
            var _web_rtc_transport_ids = default(VectorOffset);
            if(_o.WebRtcTransportIds != null)
            {
                var __web_rtc_transport_ids = new StringOffset[_o.WebRtcTransportIds.Count];
                for(var _j = 0; _j < __web_rtc_transport_ids.Length; ++_j)
                { __web_rtc_transport_ids[_j] = builder.CreateString(_o.WebRtcTransportIds[_j]); }
                _web_rtc_transport_ids = CreateWebRtcTransportIdsVector(builder, __web_rtc_transport_ids);
            }
            var _local_ice_username_fragments = default(VectorOffset);
            if(_o.LocalIceUsernameFragments != null)
            {
                var __local_ice_username_fragments = new Offset<FBS.WebRtcServer.IceUserNameFragment>[_o.LocalIceUsernameFragments.Count];
                for(var _j = 0; _j < __local_ice_username_fragments.Length; ++_j)
                { __local_ice_username_fragments[_j] = FBS.WebRtcServer.IceUserNameFragment.Pack(builder, _o.LocalIceUsernameFragments[_j]); }
                _local_ice_username_fragments = CreateLocalIceUsernameFragmentsVector(builder, __local_ice_username_fragments);
            }
            var _tuple_hashes = default(VectorOffset);
            if(_o.TupleHashes != null)
            {
                var __tuple_hashes = new Offset<FBS.WebRtcServer.TupleHash>[_o.TupleHashes.Count];
                for(var _j = 0; _j < __tuple_hashes.Length; ++_j)
                { __tuple_hashes[_j] = FBS.WebRtcServer.TupleHash.Pack(builder, _o.TupleHashes[_j]); }
                _tuple_hashes = CreateTupleHashesVector(builder, __tuple_hashes);
            }
            return CreateDumpResponse(
              builder,
              _id,
              _udp_sockets,
              _tcp_servers,
              _web_rtc_transport_ids,
              _local_ice_username_fragments,
              _tuple_hashes);
        }
    }

    public class DumpResponseT
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("udp_sockets")]
        public List<FBS.WebRtcServer.IpPortT> UdpSockets { get; set; }
        [JsonPropertyName("tcp_servers")]
        public List<FBS.WebRtcServer.IpPortT> TcpServers { get; set; }
        [JsonPropertyName("web_rtc_transport_ids")]
        public List<string> WebRtcTransportIds { get; set; }
        [JsonPropertyName("local_ice_username_fragments")]
        public List<FBS.WebRtcServer.IceUserNameFragmentT> LocalIceUsernameFragments { get; set; }
        [JsonPropertyName("tuple_hashes")]
        public List<FBS.WebRtcServer.TupleHashT> TupleHashes { get; set; }

        public DumpResponseT()
        {
            this.Id = null;
            this.UdpSockets = null;
            this.TcpServers = null;
            this.WebRtcTransportIds = null;
            this.LocalIceUsernameFragments = null;
            this.TupleHashes = null;
        }
    }
}