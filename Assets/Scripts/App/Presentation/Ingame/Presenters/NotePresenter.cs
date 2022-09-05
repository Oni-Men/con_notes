using App.Domain;
using App.Domain.Ingame;
using App.Domain.Ingame.Enums;
using App.Presentation.Ingame.Views;
using UniRx;

namespace App.Presentation.Ingame.Presenters
{
    public class NotePresenter
    {
        private NoteModel _noteModel;
        private NoteView _noteView;

        public int LaneId => _noteModel.LaneId;
        public float ZPosition => _noteView.transform.position.z;
        
        public NotePresenter(NoteView noteView)
        {
            _noteView = noteView;
        }

        public void Initialize()
        {
            var game = GameManager.GetInstance().CurrentGame;
            game?.Presenter.NotePresenters.AddNotePresenter(this);
            _noteModel = new NoteModel(_noteView.LaneId, NoteType.Normal);
            
            Bind();
        }

        private void Bind()
        {
            Observable.EveryUpdate()
                .Where(_ => ZPosition < -1)
                .Subscribe(_ => OnPassedBorder())
                .AddTo(_noteView);
        }

        private void OnPassedBorder()
        {
            var game = GameManager.GetInstance().CurrentGame;
            game?.Presenter.NotePresenters.RemoveNotePresenter(this);
            Dispose();
        }

        public void Dispose()
        {
            _noteView.Dispose();
        }
    }
}