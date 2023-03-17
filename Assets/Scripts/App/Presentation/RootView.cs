using System.Threading;
using App.Presentation.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Presentation
{
    public class RootView : MonoBehaviour
    {

        [SerializeField]
        private FadeInOutView fadeInOutView;

        private async void Start()
        {
            fadeInOutView.SetPanelBlack();
            await Initialize();
            await PlayFadeIn(CancellationToken.None);
        }

        private async UniTask Initialize()
        {
            // Load TitleScene if not loaded.
            if (SceneManager.sceneCount == 1 && SceneManager.GetActiveScene().name == PageManager.RootSceneName)
            {
                await PageManager.PushSimplyAsync("TitleScene");
            }
        }

        public async UniTask PlayFadeIn(CancellationToken ct)
        {
            await fadeInOutView.PlayFadeIn(ct);
        }

        public async UniTask PlayFadeOut(CancellationToken ct)
        {
            await fadeInOutView.PlayFadeOut(ct);
        }

    }
}