using App.Domain.Ingame.Enums;
using UnityEngine;

namespace App.Application.Ingame
{
    public readonly struct JudgementViewModel
    {
        public readonly Vector3 Position;
        public readonly JudgementType JudgementType;

        public JudgementViewModel(Vector3 position, JudgementType judgementType)
        {
            Position = position;
            JudgementType = judgementType;
        }
    }
}