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

        public static readonly IReadOnlyDictionary<JudgementType, float> JudgementByDistance = new Dictionary<JudgementType, float>()
        {
            {JudgementType.Perfect, 0.2f},
            {JudgementType.Good, 0.5f},
            {JudgementType.Bad, 1.0f},
        };
    }
}