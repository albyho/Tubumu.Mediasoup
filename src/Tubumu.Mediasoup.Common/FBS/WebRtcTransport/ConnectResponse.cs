// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.WebRtcTransport
{
    public struct ConnectResponse : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static ConnectResponse GetRootAsConnectResponse(ByteBuffer _bb) { return GetRootAsConnectResponse(_bb, new ConnectResponse()); }
        public static ConnectResponse GetRootAsConnectResponse(ByteBuffer _bb, ConnectResponse obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public ConnectResponse __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.WebRtcTransport.DtlsRole DtlsLocalRole { get { int o = __p.__offset(4); return o != 0 ? (FBS.WebRtcTransport.DtlsRole)__p.bb.Get(o + __p.bb_pos) : FBS.WebRtcTransport.DtlsRole.AUTO; } }

        public static Offset<FBS.WebRtcTransport.ConnectResponse> CreateConnectResponse(FlatBufferBuilder builder,
            FBS.WebRtcTransport.DtlsRole dtls_local_role = FBS.WebRtcTransport.DtlsRole.AUTO)
        {
            builder.StartTable(1);
            ConnectResponse.AddDtlsLocalRole(builder, dtls_local_role);
            return ConnectResponse.EndConnectResponse(builder);
        }

        public static void StartConnectResponse(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddDtlsLocalRole(FlatBufferBuilder builder, FBS.WebRtcTransport.DtlsRole dtlsLocalRole) { builder.AddByte(0, (byte)dtlsLocalRole, 0); }
        public static Offset<FBS.WebRtcTransport.ConnectResponse> EndConnectResponse(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.WebRtcTransport.ConnectResponse>(o);
        }
        public ConnectResponseT UnPack()
        {
            var _o = new ConnectResponseT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(ConnectResponseT _o)
        {
            _o.DtlsLocalRole = this.DtlsLocalRole;
        }
        public static Offset<FBS.WebRtcTransport.ConnectResponse> Pack(FlatBufferBuilder builder, ConnectResponseT _o)
        {
            if(_o == null)
                return default(Offset<FBS.WebRtcTransport.ConnectResponse>);
            return CreateConnectResponse(
              builder,
              _o.DtlsLocalRole);
        }
    }

    public class ConnectResponseT
    {
        [JsonPropertyName("dtls_local_role")]
        public FBS.WebRtcTransport.DtlsRole DtlsLocalRole { get; set; }

        public ConnectResponseT()
        {
            this.DtlsLocalRole = FBS.WebRtcTransport.DtlsRole.AUTO;
        }
    }
}
