using System.Collections.Generic;
using App.Domain.Ingame.Enums;

namespace App.Domain
{
    public static class GameConst
    {
        public const float NoteWidth = 1.1F;

        public const float Lifetime = 3.0f;

        public static readonly IReadOnlyDictionary<JudgementType, int> PointForJudge = new Dictionary<JudgementType, int>()
        {
            { JudgementType.Miss, 0 },
            { JudgementType.Bad , 1},
            { JudgementType.Good , 3},
            { JudgementType.Perfect , 5},
        };

        public static readonly IReadOnlyDictionary<JudgementType, float> JudgementThresholds = new Dictionary<JudgementType, float>()
        {
            {JudgementType.Perfect, 1f},
            {JudgementType.Good, 1.5f},
            {JudgementType.Bad, 2.0f},
        };

        public static readonly IReadOnlyDictionary<JudgementType, string> JudgementText =
            new Dictionary<JudgementType, string>()
            {
                { JudgementType.Perfect, "最高" },
                { JudgementType.Good, "良" },
                { JudgementType.Bad, "不良" },
                { JudgementType.Miss, "失敗" },
            };

        public static readonly IReadOnlyDictionary<string, int> RankToScoreMap = new Dictionary<string, int>()
        {
            {"S++", 1300 },
            {"S+", 1200 },
            {"S", 1000 },
            {"A", 800 },
            {"B", 700 },
            {"C", 1000 },
            {"D", 1000 },
            {"E", 1000 },
        };
    }
}