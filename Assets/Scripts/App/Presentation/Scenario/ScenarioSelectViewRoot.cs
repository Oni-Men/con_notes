using System.Linq;
using App.Presentation.Common;
using Cysharp.Threading.Tasks;
using Database.Impl;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Presentation.Scenario
{
    public class ScenarioSelectViewRoot : MonoBehaviour
    {
        [SerializeField]
        private PopupDialogueView popupDialogueView;

        [SerializeField]
        private ScenarioSelectView scenarioSelectView;

        private void Awake()
        {
            scenarioSelectView.ScenarioSelectEvent.Subscribe(id =>
            {
                InitScenarioView(id).Forget();
            }).AddTo(scenarioSelectView);
        }
        
        private async UniTask InitScenarioView(string scenarioID)
        {
            var scenarioData = await GitHubScenarioFetcher.Instance.Fetch(scenarioID);
            if (scenarioData == null)
            {
                await ShowPopupDialogue($"このお話は読めないようです... ({scenarioID})");
                return;
            }

            await PageManager.PushAsync("ScenarioScene", () =>
            {
                PageManager.GetComponent<ScenarioViewRoot>()?.Initialize(new ScenarioViewRoot.ScenarioViewParam()
                {
                    scenarioData = scenarioData
                });
            });
        }

        private async UniTask ShowPopupDialogue(string text)
        {
            var popupView = Instantiate(popupDialogueView);
            await popupView.ShowPopup(text, gameObject.GetCancellationTokenOnDestroy(), showButtons: false);
        }

    }
}