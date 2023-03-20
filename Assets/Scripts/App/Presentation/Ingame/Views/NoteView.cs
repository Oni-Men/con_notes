using App.Domain;
using App.Presentation.Ingame.Presenters;
using UnityEngine;

namespace App.Presentation.Ingame.Views
{
    public class NoteView : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        private NotePresenter _notePresenter;

        public void Initialize(NotePresenter notePresenter)
        {
            _notePresenter = notePresenter;

            var x = notePresenter.LaneId * GameConst.NoteWidth;
            var z = speed * GameConst.Lifetime;

            var t = transform;
            t.localPosition = new Vector3(x, 0.01f, z);
            t.rotation = Quaternion.identity;
        }

        void Update()
        {
            transform.Translate(0, 0, Time.deltaTime * -speed);
            if (transform.localPosition.z < -2)
            {
                _notePresenter.Dispose();
            }
        }
    }
}