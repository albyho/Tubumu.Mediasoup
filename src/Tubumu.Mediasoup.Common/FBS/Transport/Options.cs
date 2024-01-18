// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.Transport
{
    public struct Options : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static Options GetRootAsOptions(ByteBuffer _bb) { return GetRootAsOptions(_bb, new Options()); }
        public static Options GetRootAsOptions(ByteBuffer _bb, Options obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public Options __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public bool Direct { get { int o = __p.__offset(4); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }
        /// Only needed for DirectTransport. This value is handled by base Transport.
        public uint? MaxMessageSize { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint?)null; } }
        public uint? InitialAvailableOutgoingBitrate { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint?)null; } }
        public bool EnableSctp { get { int o = __p.__offset(10); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }
        public FBS.SctpParameters.NumSctpStreams? NumSctpStreams { get { int o = __p.__offset(12); return o != 0 ? (FBS.SctpParameters.NumSctpStreams?)(new FBS.SctpParameters.NumSctpStreams()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
        public uint MaxSctpMessageSize { get { int o = __p.__offset(14); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public uint SctpSendBufferSize { get { int o = __p.__offset(16); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public bool IsDataChannel { get { int o = __p.__offset(18); return o != 0 ? 0 != __p.bb.Get(o + __p.bb_pos) : (bool)false; } }

        public static Offset<FBS.Transport.Options> CreateOptions(FlatBufferBuilder builder,
            bool direct = false,
            uint? max_message_size = null,
            uint? initial_available_outgoing_bitrate = null,
            bool enable_sctp = false,
            Offset<FBS.SctpParameters.NumSctpStreams> num_sctp_streamsOffset = default(Offset<FBS.SctpParameters.NumSctpStreams>),
            uint max_sctp_message_size = 0,
            uint sctp_send_buffer_size = 0,
            bool is_data_channel = false)
        {
            builder.StartTable(8);
            Options.AddSctpSendBufferSize(builder, sctp_send_buffer_size);
            Options.AddMaxSctpMessageSize(builder, max_sctp_message_size);
            Options.AddNumSctpStreams(builder, num_sctp_streamsOffset);
            Options.AddInitialAvailableOutgoingBitrate(builder, initial_available_outgoing_bitrate);
            Options.AddMaxMessageSize(builder, max_message_size);
            Options.AddIsDataChannel(builder, is_data_channel);
            Options.AddEnableSctp(builder, enable_sctp);
            Options.AddDirect(builder, direct);
            return Options.EndOptions(builder);
        }

        public static void StartOptions(FlatBufferBuilder builder) { builder.StartTable(8); }
        public static void AddDirect(FlatBufferBuilder builder, bool direct) { builder.AddBool(0, direct, false); }
        public static void AddMaxMessageSize(FlatBufferBuilder builder, uint? maxMessageSize) { builder.AddUint(1, maxMessageSize); }
        public static void AddInitialAvailableOutgoingBitrate(FlatBufferBuilder builder, uint? initialAvailableOutgoingBitrate) { builder.AddUint(2, initialAvailableOutgoingBitrate); }
        public static void AddEnableSctp(FlatBufferBuilder builder, bool enableSctp) { builder.AddBool(3, enableSctp, false); }
        public static void AddNumSctpStreams(FlatBufferBuilder builder, Offset<FBS.SctpParameters.NumSctpStreams> numSctpStreamsOffset) { builder.AddOffset(4, numSctpStreamsOffset.Value, 0); }
        public static void AddMaxSctpMessageSize(FlatBufferBuilder builder, uint maxSctpMessageSize) { builder.AddUint(5, maxSctpMessageSize, 0); }
        public static void AddSctpSendBufferSize(FlatBufferBuilder builder, uint sctpSendBufferSize) { builder.AddUint(6, sctpSendBufferSize, 0); }
        public static void AddIsDataChannel(FlatBufferBuilder builder, bool isDataChannel) { builder.AddBool(7, isDataChannel, false); }
        public static Offset<FBS.Transport.Options> EndOptions(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<FBS.Transport.Options>(o);
        }
        public OptionsT UnPack()
        {
            var _o = new OptionsT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(OptionsT _o)
        {
            _o.Direct = this.Direct;
            _o.MaxMessageSize = this.MaxMessageSize;
            _o.InitialAvailableOutgoingBitrate = this.InitialAvailableOutgoingBitrate;
            _o.EnableSctp = this.EnableSctp;
            _o.NumSctpStreams = this.NumSctpStreams.HasValue ? this.NumSctpStreams.Value.UnPack() : null;
            _o.MaxSctpMessageSize = this.MaxSctpMessageSize;
            _o.SctpSendBufferSize = this.SctpSendBufferSize;
            _o.IsDataChannel = this.IsDataChannel;
        }
        public static Offset<FBS.Transport.Options> Pack(FlatBufferBuilder builder, OptionsT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Transport.Options>);
            var _num_sctp_streams = _o.NumSctpStreams == null ? default(Offset<FBS.SctpParameters.NumSctpStreams>) : FBS.SctpParameters.NumSctpStreams.Pack(builder, _o.NumSctpStreams);
            return CreateOptions(
              builder,
              _o.Direct,
              _o.MaxMessageSize,
              _o.InitialAvailableOutgoingBitrate,
              _o.EnableSctp,
              _num_sctp_streams,
              _o.MaxSctpMessageSize,
              _o.SctpSendBufferSize,
              _o.IsDataChannel);
        }
    }

    public class OptionsT
    {
        [JsonPropertyName("direct")]
        public bool Direct { get; set; }
        [JsonPropertyName("max_message_size")]
        public uint? MaxMessageSize { get; set; }
        [JsonPropertyName("initial_available_outgoing_bitrate")]
        public uint? InitialAvailableOutgoingBitrate { get; set; }
        [JsonPropertyName("enable_sctp")]
        public bool EnableSctp { get; set; }
        [JsonPropertyName("num_sctp_streams")]
        public FBS.SctpParameters.NumSctpStreamsT NumSctpStreams { get; set; }
        [JsonPropertyName("max_sctp_message_size")]
        public uint MaxSctpMessageSize { get; set; }
        [JsonPropertyName("sctp_send_buffer_size")]
        public uint SctpSendBufferSize { get; set; }
        [JsonPropertyName("is_data_channel")]
        public bool IsDataChannel { get; set; }

        public OptionsT()
        {
            this.Direct = false;
            this.MaxMessageSize = null;
            this.InitialAvailableOutgoingBitrate = null;
            this.EnableSctp = false;
            this.NumSctpStreams = null;
            this.MaxSctpMessageSize = 0;
            this.SctpSendBufferSize = 0;
            this.IsDataChannel = false;
        }
    }
}
