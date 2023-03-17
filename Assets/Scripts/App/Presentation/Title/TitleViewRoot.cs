using System.Threading;
using App.Domain;
using App.Presentation.Common;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Title
{
    public class TitleViewRoot : MonoBehaviour
    {
        [SerializeField]
        private Button startButton;

        [SerializeField]
        private Button exitButton;

        [SerializeField]
        private FadeInOutView fader;

        void Awake()
        {
            GameConst.LoadMasterData();

            startButton.OnClickAsObservable().Subscribe(_ => ShowScenarioSelectScene().Forget()).AddTo(this);
            exitButton.OnClickAsObservable().Subscribe(_ => ExitGame().Forget()).AddTo(this);

            GameManager.ShouldPlayCutIn = false;
        }

        private async UniTask ShowScenarioSelectScene()
        {
            await fader.PlayFadeOut(CancellationToken.None);
            await PageManager.PushAsync("ScenarioSelectScene");
        }

        private async UniTask ExitGame()
        {
            await fader.PlayFadeOut(CancellationToken.None);
            UnityEngine.Application.Quit();
        }
    }
}