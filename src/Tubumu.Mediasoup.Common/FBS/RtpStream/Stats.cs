// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;
using Google.FlatBuffers;

namespace FBS.RtpStream
{
    public struct Stats : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
        public static Stats GetRootAsStats(ByteBuffer _bb) { return GetRootAsStats(_bb, new Stats()); }
        public static Stats GetRootAsStats(ByteBuffer _bb, Stats obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public Stats __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.RtpStream.StatsData DataType { get { int o = __p.__offset(4); return o != 0 ? (FBS.RtpStream.StatsData)__p.bb.Get(o + __p.bb_pos) : FBS.RtpStream.StatsData.NONE; } }
        public TTable? Data<TTable>() where TTable : struct, IFlatbufferObject { int o = __p.__offset(6); return o != 0 ? (TTable?)__p.__union<TTable>(o + __p.bb_pos) : null; }
        public FBS.RtpStream.BaseStats DataAsBaseStats() { return Data<FBS.RtpStream.BaseStats>().Value; }
        public FBS.RtpStream.RecvStats DataAsRecvStats() { return Data<FBS.RtpStream.RecvStats>().Value; }
        public FBS.RtpStream.SendStats DataAsSendStats() { return Data<FBS.RtpStream.SendStats>().Value; }

        public static Offset<FBS.RtpStream.Stats> CreateStats(FlatBufferBuilder builder,
            FBS.RtpStream.StatsData data_type = FBS.RtpStream.StatsData.NONE,
            int dataOffset = 0)
        {
            builder.StartTable(2);
            Stats.AddData(builder, dataOffset);
            Stats.AddDataType(builder, data_type);
            return Stats.EndStats(builder);
        }

        public static void StartStats(FlatBufferBuilder builder) { builder.StartTable(2); }
        public static void AddDataType(FlatBufferBuilder builder, FBS.RtpStream.StatsData dataType) { builder.AddByte(0, (byte)dataType, 0); }
        public static void AddData(FlatBufferBuilder builder, int dataOffset) { builder.AddOffset(1, dataOffset, 0); }
        public static Offset<FBS.RtpStream.Stats> EndStats(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 6);  // data
            return new Offset<FBS.RtpStream.Stats>(o);
        }
        public StatsT UnPack()
        {
            var _o = new StatsT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(StatsT _o)
        {
            _o.Data = new FBS.RtpStream.StatsDataUnion();
            _o.Data.Type = this.DataType;
            switch(this.DataType)
            {
                default:
                    break;
                case FBS.RtpStream.StatsData.BaseStats:
                    _o.Data.Value = this.Data<FBS.RtpStream.BaseStats>().HasValue ? this.Data<FBS.RtpStream.BaseStats>().Value.UnPack() : null;
                    break;
                case FBS.RtpStream.StatsData.RecvStats:
                    _o.Data.Value = this.Data<FBS.RtpStream.RecvStats>().HasValue ? this.Data<FBS.RtpStream.RecvStats>().Value.UnPack() : null;
                    break;
                case FBS.RtpStream.StatsData.SendStats:
                    _o.Data.Value = this.Data<FBS.RtpStream.SendStats>().HasValue ? this.Data<FBS.RtpStream.SendStats>().Value.UnPack() : null;
                    break;
            }
        }
        public static Offset<FBS.RtpStream.Stats> Pack(FlatBufferBuilder builder, StatsT _o)
        {
            if(_o == null)
                return default(Offset<FBS.RtpStream.Stats>);
            var _data_type = _o.Data == null ? FBS.RtpStream.StatsData.NONE : _o.Data.Type;
            var _data = _o.Data == null ? 0 : FBS.RtpStream.StatsDataUnion.Pack(builder, _o.Data);
            return CreateStats(
              builder,
              _data_type,
              _data);
        }
    }

    public class StatsT
    {
        [JsonPropertyName("data_type")]
        private FBS.RtpStream.StatsData DataType
        {
            get
            {
                return this.Data != null ? this.Data.Type : FBS.RtpStream.StatsData.NONE;
            }
            set
            {
                this.Data = new FBS.RtpStream.StatsDataUnion();
                this.Data.Type = value;
            }
        }
        [JsonPropertyName("data")]
        [JsonConverter(typeof(FBS.RtpStream.RtpStreamStatsDataUnionJsonConverter))]
        public FBS.RtpStream.StatsDataUnion Data { get; set; }

        public StatsT()
        {
            this.Data = null;
        }
    }
}
