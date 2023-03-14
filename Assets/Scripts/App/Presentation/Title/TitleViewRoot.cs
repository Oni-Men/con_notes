using System.Linq;
using System.Threading;
using App.Domain;
using App.Presentation.Common;
using App.Presentation.Ingame.Views;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
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

            startButton.OnClickAsObservable().Subscribe(_ => ShowInGameScene().Forget()).AddTo(this);
            exitButton.OnClickAsObservable().Subscribe(_ => ExitGame().Forget()).AddTo(this);

            GameManager.ShouldPlayCutIn = true;
        }

        private async UniTask ShowInGameScene()
        {
            await fader.PlayFadeOut(CancellationToken.None);
            SceneManager.LoadScene("ScenarioSelectScene");
            var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            var inGameViewRoot = rootGameObjects
                .Select(go => go.GetComponent<InGameViewRoot>())
                .FirstOrDefault(view => view is not null);

            if (inGameViewRoot is null)
            {
                return;
            }

            var param = new InGameViewRoot.InGameViewParam{};
            await inGameViewRoot.Initialize(param);
        }

        private async UniTask ExitGame()
        {
            await fader.PlayFadeOut(CancellationToken.None);
            UnityEngine.Application.Quit();
        }
    }
}