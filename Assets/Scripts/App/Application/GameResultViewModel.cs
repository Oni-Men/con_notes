using System.Collections.Generic;
using App.Domain;
using App.Domain.Ingame;
using App.Domain.Ingame.Enums;

namespace App.Application
{
    public class GameResultViewModel
    {
        public readonly string songDirectoryPath;
        public readonly int score;
        public readonly int maxCombo;
        public readonly string rankText;
        public readonly bool isSucceed;
        public readonly IReadOnlyDictionary<JudgementType, int> evalCounts;

        public GameResultViewModel(GameData gameData)
        {
            songDirectoryPath = gameData.SongDirectoryPath;
            score = gameData.Score.Value;
            maxCombo = gameData.MaxCombo.Value;
            rankText = GameConst.GetRankText(score);
            isSucceed = gameData.IsAlive;
            evalCounts = new Dictionary<JudgementType, int>(gameData.EvalCounts);
        }
    }
}