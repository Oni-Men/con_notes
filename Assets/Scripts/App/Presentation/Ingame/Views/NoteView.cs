using App.Domain;
using App.Presentation.Ingame.Presenters;
using UnityEngine;

namespace App.Presentation.Ingame.Views
{
    public class NoteView : MonoBehaviour
    {
        [SerializeField] private float speed;
        
        public NotePresenter Presenter { get; private set; }

        public float Speed => speed;

        public int LaneId => (int) (transform.position.x / GameConst.NoteWidth);
        
        void Awake()
        {
            Presenter = new NotePresenter(this);
            Presenter.Initialize();
        }

        void Update()
        {
            transform.Translate(0, 0, Time.deltaTime * -speed);
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}
