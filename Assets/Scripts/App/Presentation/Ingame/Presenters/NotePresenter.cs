using System;
using App.Domain;
using App.Domain.Ingame.Enums;
using App.Presentation.Ingame.Views;

namespace App.Presentation.Ingame.Presenters
{
    public class NotePresenter : IDisposable
    {
        private NoteView _noteView;

        public JudgementType Judgement { get; private set; }
        
        public int LaneId { get; private set; }

        public float ZPosition => _noteView.transform.position.z;
        private GamePresenter _gamePresenter;
        private NotePresenters _notePresenters;
        
        public void Initialize(GamePresenter gamePresenter, NotePresenters notePresenters, NoteView noteView, int laneId)
        {
            LaneId = laneId;
            Judgement = JudgementType.NotJudged;
            
            _gamePresenter = gamePresenter;
            _notePresenters = notePresenters;
            _noteView = noteView;
            notePresenters.AddNotePresenter(this);
            noteView.Initialize(this);
        }

        public JudgementType Judge(float distance)
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

        public void Dispose()
        {
            _notePresenters.RemoveNotePresenter(this);

            if (Judgement == JudgementType.NotJudged)
            {
                _gamePresenter.OnNotePassed(this);
                Judgement = JudgementType.Miss;
            }

            _gamePresenter.DespawnNote(_noteView);
        }
    }
}