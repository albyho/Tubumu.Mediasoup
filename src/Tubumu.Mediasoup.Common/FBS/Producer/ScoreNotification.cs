// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.Producer
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

        public FBS.Producer.Score? Scores(int j) { int o = __p.__offset(4); return o != 0 ? (FBS.Producer.Score?)(new FBS.Producer.Score()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
        public int ScoresLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }

        public static Offset<FBS.Producer.ScoreNotification> CreateScoreNotification(FlatBufferBuilder builder,
            VectorOffset scoresOffset = default(VectorOffset))
        {
            builder.StartTable(1);
            ScoreNotification.AddScores(builder, scoresOffset);
            return ScoreNotification.EndScoreNotification(builder);
        }

        public static void StartScoreNotification(FlatBufferBuilder builder) { builder.StartTable(1); }
        public static void AddScores(FlatBufferBuilder builder, VectorOffset scoresOffset) { builder.AddOffset(0, scoresOffset.Value, 0); }
        public static VectorOffset CreateScoresVector(FlatBufferBuilder builder, Offset<FBS.Producer.Score>[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateScoresVectorBlock(FlatBufferBuilder builder, Offset<FBS.Producer.Score>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateScoresVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<FBS.Producer.Score>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateScoresVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<FBS.Producer.Score>>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartScoresVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static Offset<FBS.Producer.ScoreNotification> EndScoreNotification(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // scores
            return new Offset<FBS.Producer.ScoreNotification>(o);
        }
        public ScoreNotificationT UnPack()
        {
            var _o = new ScoreNotificationT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(ScoreNotificationT _o)
        {
            _o.Scores = new List<FBS.Producer.ScoreT>();
            for(var _j = 0; _j < this.ScoresLength; ++_j)
            { _o.Scores.Add(this.Scores(_j).HasValue ? this.Scores(_j).Value.UnPack() : null); }
        }
        public static Offset<FBS.Producer.ScoreNotification> Pack(FlatBufferBuilder builder, ScoreNotificationT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Producer.ScoreNotification>);
            var _scores = default(VectorOffset);
            if(_o.Scores != null)
            {
                var __scores = new Offset<FBS.Producer.Score>[_o.Scores.Count];
                for(var _j = 0; _j < __scores.Length; ++_j)
                { __scores[_j] = FBS.Producer.Score.Pack(builder, _o.Scores[_j]); }
                _scores = CreateScoresVector(builder, __scores);
            }
            return CreateScoreNotification(
              builder,
              _scores);
        }
    }

    public class ScoreNotificationT
    {
        public List<FBS.Producer.ScoreT> Scores { get; set; }

        public ScoreNotificationT()
        {
            this.Scores = null;
        }
    }


    static public class ScoreNotificationVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyVectorOfTables(tablePos, 4 /*Scores*/, FBS.Producer.ScoreVerify.Verify, true)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
