using App.Domain;
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

        void Awake()
        {
            GameConst.LoadMasterData();

            startButton.OnClickAsObservable().Subscribe(_ => ShowScenarioSelectScene().Forget()).AddTo(this);
            exitButton.OnClickAsObservable().Subscribe(_ => ExitGame().Forget()).AddTo(this);

            GameManager.ShouldPlayCutIn = false;
        }

        private async UniTask ShowScenarioSelectScene()
        {
            await PageManager.PushAsyncWithFade("ScenarioSelectScene");
        }

        private async UniTask ExitGame()
        {
            await PageManager.PopAsync();
        }
    }
}