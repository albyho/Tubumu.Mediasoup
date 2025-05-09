// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.Consumer
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct ScoreNotification : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static ScoreNotification GetRootAsScoreNotification(ByteBuffer _bb) { return GetRootAsScoreNotification(_bb, new ScoreNotification()); }
        public static ScoreNotification GetRootAsScoreNotification(ByteBuffer _bb, ScoreNotification obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public ScoreNotification __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public FBS.Consumer.ConsumerScore? Score { get { int o = __p.__offset(4); return o != 0 ? (FBS.Consumer.ConsumerScore?)(new FBS.Consumer.ConsumerScore()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }

        public static Offset<FBS.Consumer.ScoreNotification> CreateScoreNotification(FlatBufferBuilder builder,
            Offset<FBS.Consumer.ConsumerScore> scoreOffset = default(Offset<FBS.Consumer.ConsumerScore>))
        {
            builder.StartTable(1);
            ScoreNotification.AddScore(builder, scoreOffset);
            return ScoreNotification.EndScoreNotification(builder);
        }

        public static void StartScoreNotification(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddScore(FlatBufferBuilder builder, Offset<FBS.Consumer.ConsumerScore> scoreOffset) { builder.AddOffset(0, scoreOffset.Value, 0); }
        public static Offset<FBS.Consumer.ScoreNotification> EndScoreNotification(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // score
            return new Offset<FBS.Consumer.ScoreNotification>(o);
        }
        public ScoreNotificationT UnPack()
        {
            var _o = new ScoreNotificationT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(ScoreNotificationT _o)
        {
            _o.Score = this.Score.HasValue ? this.Score.Value.UnPack() : null;
        }
        public static Offset<FBS.Consumer.ScoreNotification> Pack(FlatBufferBuilder builder, ScoreNotificationT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Consumer.ScoreNotification>);
            var _score = _o.Score == null ? default(Offset<FBS.Consumer.ConsumerScore>) : FBS.Consumer.ConsumerScore.Pack(builder, _o.Score);
            return CreateScoreNotification(
              builder,
              _score);
        }
    }

    public class ScoreNotificationT
    {
        public FBS.Consumer.ConsumerScoreT Score { get; set; }

        public ScoreNotificationT()
        {
            this.Score = null;
        }
    }


    static public class ScoreNotificationVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyTable(tablePos, 4 /*Score*/, FBS.Consumer.ConsumerScoreVerify.Verify, true)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
