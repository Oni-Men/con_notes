using App.Domain.Ingame.Enums;
using UnityEngine;

namespace App.Application.Ingame
{
    public readonly struct JudgementViewModel
    {
        public readonly int LaneId;
        public readonly JudgementType JudgementType;

        public JudgementViewModel(int laneId, JudgementType judgementType)
        {
            LaneId = laneId;
            JudgementType = judgementType;
        }
    }
}