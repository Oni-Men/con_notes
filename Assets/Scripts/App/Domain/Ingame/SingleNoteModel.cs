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
            if (distance < GameConst.JudgementThresholds[JudgementType.Perfect])
            {
                return JudgementType.Perfect;
            }

            if (distance < GameConst.JudgementThresholds[JudgementType.Good])
            {
                return JudgementType.Good;
            }

            if (distance < GameConst.JudgementThresholds[JudgementType.Bad])
            {
                return JudgementType.Bad;
            }

            return JudgementType.Miss;
        }
    }
}