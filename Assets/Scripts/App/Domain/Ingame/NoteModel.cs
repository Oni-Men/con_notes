using App.Domain.Ingame.Enums;

namespace App.Domain.Ingame
{
    public class NoteModel
    {
        public int LaneId { get; }
        public NoteType NoteType { get; }

        public JudgementType Judgement;
        
        public NoteModel(int laneId, NoteType noteType)
        {
            LaneId = laneId;
            NoteType = noteType;
            Judgement = JudgementType.NotJudged;
        }
        
    }
}