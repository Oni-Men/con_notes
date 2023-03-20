using UnityEngine;
using UnityEngine.Pool;

namespace App.Presentation.Ingame.Views
{
    public class NoteViewPool : MonoBehaviour
    {
        [SerializeField]
        private NoteView notePrefab;

        private ObjectPool<NoteView> notesPool;
        
        public void Initialize()
        {
            notesPool = new ObjectPool<NoteView>(CreateFunc, GetFunc, ReleaseFunc, DestroyFunc, defaultCapacity: 50);
        }
        
        public NoteView GetNoteView()
        {
            return notesPool.Get();
        }

        public void ReleaseNoteView(NoteView noteView)
        {
            notesPool.Release(noteView);
        }
        
        private NoteView CreateFunc()
        {
            var noteView = Instantiate(notePrefab, transform);
            return noteView;
        }

        private void GetFunc(NoteView noteView)
        {
            noteView.gameObject.SetActive(true);
        }

        private void ReleaseFunc(NoteView noteView)
        {
            noteView.gameObject.SetActive(false);
        }

        private void DestroyFunc(NoteView noteView)
        {
            Destroy(noteView.gameObject);
        }
    }
}