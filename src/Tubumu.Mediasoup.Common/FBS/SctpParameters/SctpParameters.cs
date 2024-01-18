// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using Google.FlatBuffers;
using System.Text.Json.Serialization;

namespace FBS.SctpParameters
{
    public struct SctpParameters : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static SctpParameters GetRootAsSctpParameters(ByteBuffer _bb) { return GetRootAsSctpParameters(_bb, new SctpParameters()); }
        public static SctpParameters GetRootAsSctpParameters(ByteBuffer _bb, SctpParameters obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public SctpParameters __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public ushort Port { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUshort(o + __p.bb_pos) : (ushort)5000; } }
        public ushort Os { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUshort(o + __p.bb_pos) : (ushort)0; } }
        public ushort Mis { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetUshort(o + __p.bb_pos) : (ushort)0; } }
        public uint MaxMessageSize { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public uint SendBufferSize { get { int o = __p.__offset(12); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public uint SctpBufferedAmount { get { int o = __p.__offset(14); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public bool IsDataChannel { get { int o = __p.__offset(16); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }

        public static Offset<FBS.SctpParameters.SctpParameters> CreateSctpParameters(FlatBufferBuilder builder,
            ushort port = 5000,
            ushort os = 0,
            ushort mis = 0,
            uint max_message_size = 0,
            uint send_buffer_size = 0,
            uint sctp_buffered_amount = 0,
            bool is_data_channel = false)
        {
            builder.StartTable(7);
            SctpParameters.AddSctpBufferedAmount(builder, sctp_buffered_amount);
            SctpParameters.AddSendBufferSize(builder, send_buffer_size);
            SctpParameters.AddMaxMessageSize(builder, max_message_size);
            SctpParameters.AddMis(builder, mis);
            SctpParameters.AddOs(builder, os);
            SctpParameters.AddPort(builder, port);
            SctpParameters.AddIsDataChannel(builder, is_data_channel);
            return SctpParameters.EndSctpParameters(builder);
        }

        public static void StartSctpParameters(FlatBufferBuilder builder) { builder.StartTable(7); }
        public static void AddPort(FlatBufferBuilder builder, ushort port) { builder.AddUshort(0, port, 5000); }
        public static void AddOs(FlatBufferBuilder builder, ushort os) { builder.AddUshort(1, os, 0); }
        public static void AddMis(FlatBufferBuilder builder, ushort mis) { builder.AddUshort(2, mis, 0); }
        public static void AddMaxMessageSize(FlatBufferBuilder builder, uint maxMessageSize) { builder.AddUint(3, maxMessageSize, 0); }
        public static void AddSendBufferSize(FlatBufferBuilder builder, uint sendBufferSize) { builder.AddUint(4, sendBufferSize, 0); }
        public static void AddSctpBufferedAmount(FlatBufferBuilder builder, uint sctpBufferedAmount) { builder.AddUint(5, sctpBufferedAmount, 0); }
        public static void AddIsDataChannel(FlatBufferBuilder builder, bool isDataChannel) { builder.AddBool(6, isDataChannel, false); }
        public static Offset<FBS.SctpParameters.SctpParameters> EndSctpParameters(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.SctpParameters.SctpParameters>(o);
        }
        public SctpParametersT UnPack()
        {
            var _o = new SctpParametersT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(SctpParametersT _o)
        {
            _o.Port = this.Port;
            _o.Os = this.Os;
            _o.Mis = this.Mis;
            _o.MaxMessageSize = this.MaxMessageSize;
            _o.SendBufferSize = this.SendBufferSize;
            _o.SctpBufferedAmount = this.SctpBufferedAmount;
            _o.IsDataChannel = this.IsDataChannel;
        }
        public static Offset<FBS.SctpParameters.SctpParameters> Pack(FlatBufferBuilder builder, SctpParametersT _o)
        {
            if(_o == null)
                return default(Offset<FBS.SctpParameters.SctpParameters>);
            return CreateSctpParameters(
              builder,
              _o.Port,
              _o.Os,
              _o.Mis,
              _o.MaxMessageSize,
              _o.SendBufferSize,
              _o.SctpBufferedAmount,
              _o.IsDataChannel);
        }
    }

    public class SctpParametersT
    {
        [JsonPropertyName("port")]
        public ushort Port { get; set; }
        /// <summary>
        /// OS. Renamed.
        /// </summary>
        [JsonPropertyName("OS")]
        public ushort Os { get; set; }
        /// <summary>
        /// MIS. Renamed.
        /// </summary>
        [JsonPropertyName("MIS")]
        public ushort Mis { get; set; }
        [JsonPropertyName("max_message_size")]
        public uint MaxMessageSize { get; set; }
        [JsonPropertyName("send_buffer_size")]
        public uint SendBufferSize { get; set; }
        [JsonPropertyName("sctp_buffered_amount")]
        public uint SctpBufferedAmount { get; set; }
        [JsonPropertyName("is_data_channel")]
        public bool IsDataChannel { get; set; }

        public SctpParametersT()
        {
            this.Port = 5000;
            this.Os = 0;
            this.Mis = 0;
            this.MaxMessageSize = 0;
            this.SendBufferSize = 0;
            this.SctpBufferedAmount = 0;
            this.IsDataChannel = false;
        }
    }
}
