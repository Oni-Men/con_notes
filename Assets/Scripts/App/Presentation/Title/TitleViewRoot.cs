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
        [SerializeField] private Button startButton;

        [SerializeField] private Button exitButton;

        [SerializeField] private PopupDialogueView instructionView;

        void Awake()
        {
            GameConst.LoadMasterData();

            startButton.OnClickAsObservable().Subscribe(_ => ShowScenarioSelectScene().Forget()).AddTo(this);
            exitButton.OnClickAsObservable().Subscribe(_ => ExitGame().Forget()).AddTo(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitGame().Forget();
            }
        }

        private async UniTask ShowScenarioSelectScene()
        {
            await PageManager.PushAsyncWithFade("ScenarioSelectScene");
        }

        private async UniTask ExitGame()
        {
            await PageManager.PopAsync();
        }

        public void OnInstructionButtonClick()
        {
            var text = @"シナリオを読み進めると画面が暗くなり、ゲーム（音ゲー）が始まります。
操作は「←、↓、→」の三つの矢印キーで行います。
奥からやってくるノートが手前のバーに当たる瞬間にあわせてタイミングよく矢印キーをクリックしてください！";
            instructionView.ShowPopup(text, CancellationToken.None, showButtons: false).Forget();
        }
    }
}