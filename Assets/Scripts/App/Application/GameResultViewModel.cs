using System.Collections.Generic;
using App.Domain;
using App.Domain.Ingame;
using App.Domain.Ingame.Enums;

namespace App.Application
{
    public class GameResultViewModel
    {
        public readonly int Score;
        public readonly int MaxCombo;
        public readonly string RankText;
        public readonly bool IsSucceed;
        public IReadOnlyDictionary<JudgementType, int> EvalCounts;

        public GameResultViewModel(GameModel gameModel)
        {
            Score = gameModel.Score.Value;
            MaxCombo = gameModel.MaxCombo.Value;
            RankText = GameConst.GetRankText(Score);
            IsSucceed = gameModel.IsAlive;
            EvalCounts = new Dictionary<JudgementType, int>(gameModel.EvalCounts);
        }
    }
}