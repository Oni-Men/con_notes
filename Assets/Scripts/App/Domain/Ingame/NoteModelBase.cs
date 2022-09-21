using App.Domain.Ingame.Enums;
using App.Domain.Notes;
using UnityEngine;

namespace App.Domain.Ingame
{
    public abstract class NoteModelBase
    {
        public int LaneId { get; }
        public NoteType NoteType { get; }

        public JudgementType Judgement;

        public readonly float PassJudgeBarAt;
        
        public NoteModelBase(int laneId, NoteType noteType)
        {
            LaneId = laneId;
            NoteType = noteType;
            Judgement = JudgementType.NotJudged;
            PassJudgeBarAt = Time.realtimeSinceStartup + GameConst.Lifetime;
        }

        public virtual void OnPassedJudgementBar() {}
        public abstract JudgementType Judge(float distance);
    }
}