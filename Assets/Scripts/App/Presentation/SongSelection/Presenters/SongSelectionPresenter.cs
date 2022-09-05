using App.Domain;
using App.Domain.SongSelection;
using App.Presentation.SongSelection.Views;

namespace App.Presentation.SongSelection.Presenters
{
    public class SongSelectionPresenter
    {
        private SongSelectionRootView _view;
        private SongSelectionModel _model;

        public SongSelectionPresenter(SongSelectionRootView view)
        {
            _view = view;
            _model = SongSelectionModel.GetInstance();
        }

        public void Initialize()
        {
        }

        public void Bind()
        {
            
        }
        
    }
}