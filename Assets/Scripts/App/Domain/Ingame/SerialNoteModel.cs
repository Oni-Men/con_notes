using App.Domain.Ingame.Enums;
using App.Domain.Notes;

namespace App.Domain.Ingame
{
    public class SerialNoteModel : NoteModelBase
    {
        public SerialNoteModel(int laneId) : base(laneId, NoteType.Serial)
        {
        }

        public override void OnPassedJudgementBar()
        {
        }

        public override JudgementType Judge(float distance)
        {
            return JudgementType.Perfect;
        }
    }
}