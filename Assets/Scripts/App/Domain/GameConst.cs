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
                { JudgementType.Perfect, "秀" },
                { JudgementType.Good, "優" },
                { JudgementType.Bad, "可" },
                { JudgementType.Miss, "不可" },
            };

        public static readonly IReadOnlyDictionary<string, int> RankToScoreMap = new Dictionary<string, int>()
        {
            {"秀+", 21000 },
            {"秀", 17000 },
            {"優+", 13000 },
            {"優", 9000 },
            {"可+", 5000 },
            {"可", 1000 },
            {"不可", 0 },
        };
    }
}