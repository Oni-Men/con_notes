using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace App.Presentation.Title
{
    public class TitleViewRoot : MonoBehaviour
    {
        [SerializeField] private Button startButton;

        [SerializeField] private Button extiButton;
    
        void Awake()
        {
            startButton.OnClickAsObservable().Subscribe(_ => ShowIngameScene()).AddTo(this);
            extiButton.OnClickAsObservable().Subscribe(_ => ExitGame()).AddTo(this);
        
            LoadBeatmaps();
        }

        private void LoadBeatmaps()
        {
            var playableAssets = Resources.LoadAll<TimelineAsset>("Songs");

            foreach (var playableAsset in playableAssets)
            {
                Debug.Log(playableAsset.name);
            }
        }
    
        private void ShowSongSelectionScene() {}

        private void ShowIngameScene()
        {
            SceneManager.LoadScene("IngameScene");
        }

        private void ExitGame()
        {
            //TODO Impl exit process.
            UnityEditor.EditorApplication.isPlaying = false;
            UnityEngine.Application.Quit();
        }
    }
}
