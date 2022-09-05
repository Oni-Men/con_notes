using App.Presentation.SongSelection.Presenters;
using UnityEngine;

namespace App.Presentation.SongSelection.Views
{
    public class SongSelectionRootView : MonoBehaviour
    {

        private SongSelectionPresenter _presenter;
        
        void Awake()
        {
            _presenter = new SongSelectionPresenter(this);
        }

        void Update()
        {
        
        }
    }
}
