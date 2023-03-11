using App.Presentation.Common;
using Cysharp.Threading.Tasks;
using Database.Impl;
using UniRx;
using UnityEngine;

namespace App.Presentation.Scenario
{
    public class ScenarioViewRoot : MonoBehaviour
    {
        [SerializeField]
        private ScenarioView scenarioView;

        [SerializeField]
        private ScenarioSelectView scenarioSelectView;

        [SerializeField]
        private PopupDialogueView popupDialogueView;
        

        private void Start()
        {
            ShowScenarioSelectView();
            scenarioSelectView.ScenarioSelectEvent.Subscribe(id =>
            {
                InitScenarioView(id).Forget();
            }).AddTo(scenarioSelectView);
        }

        private async UniTask InitScenarioView(string scenarioID)
        {
            var scenarioRoot = await GitHubScenarioFetcher.Instance.Fetch(scenarioID);
            if (scenarioRoot == null)
            {
                await ShowPopupDialogue($"このお話は読めないようです... (第{scenarioID}号)");
                return;
            }

            ShowScenarioView();
            scenarioView.Initialize(scenarioRoot);
        }

        private void ShowScenarioView()
        {
            scenarioView.gameObject.SetActive(true);
            scenarioSelectView.gameObject.SetActive(false);
        }

        private void ShowScenarioSelectView()
        {
            scenarioView.gameObject.SetActive(false);
            scenarioSelectView.gameObject.SetActive(true);
        }

        private async UniTask ShowPopupDialogue(string text)
        {
            var popupView = Instantiate(popupDialogueView);
            await popupView.ShowPopup(text, gameObject.GetCancellationTokenOnDestroy(), showButtons: false);
        }
    }
}