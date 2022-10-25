using App.Domain;
using App.Domain.Ingame;
using App.Domain.Ingame.Enums;
using App.Domain.Notes;
using App.Presentation.Ingame.Views;
using UniRx;

namespace App.Presentation.Ingame.Presenters
{
    public class NotePresenter
    {
        private NoteModelBase _noteModelBase;
        private NoteView _noteView;

        public int LaneId => _noteModelBase.LaneId;
        public float ZPosition => _noteView.transform.position.z;
        public NoteType Type => _noteModelBase.NoteType;
        public NotePresenter(NoteView noteView)
        {
            _noteView = noteView;
        }

        public void Initialize()
        {
            var game = GameManager.GetInstance().CurrentGame;
            game?.Presenter.NotePresenters.AddNotePresenter(this);
            _noteModelBase = new SingleNoteModelBase(_noteView.LaneId);
            Bind();
        }

        private void Bind()
        {
            Observable.EveryUpdate()
                .Where(_ => ZPosition < -2)
                .Subscribe(_ => Dispose())
                .AddTo(_noteView);
        }

        public JudgementType Judge(float distance)
        {
            return _noteModelBase.Judge(distance);
        }

        private void Dispose()
        {
            var game = GameManager.GetInstance().CurrentGame;
            game?.Presenter.NotePresenters.RemoveNotePresenter(this);

            if (_noteModelBase.Judgement == JudgementType.NotJudged)
            {
                game?.ProcessPassedNote(this);
            }
            _noteView.Dispose();
        }
    }
}