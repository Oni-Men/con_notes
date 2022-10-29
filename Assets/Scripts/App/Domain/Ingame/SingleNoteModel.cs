using App.Domain.Ingame.Enums;
using App.Domain.Notes;

namespace App.Domain.Ingame
{
    public class SingleNoteModelBase : NoteModelBase
    {
        public SingleNoteModelBase(int laneId) : base(laneId, NoteType.Single)
        {
        }

        public override JudgementType Judge(float distance)
        {
            Judgement =  JudgementType.Miss;

            if (distance < GameConst.EvalAndThresholds[JudgementType.Bad])
            {
                Judgement =  JudgementType.Bad;
            }

            if (distance < GameConst.EvalAndThresholds[JudgementType.Good])
            {
                Judgement =  JudgementType.Good;
            }
            
            if (distance < GameConst.EvalAndThresholds[JudgementType.Perfect])
            {
                Judgement = JudgementType.Perfect;
            }

            return Judgement;
        }
    }
}