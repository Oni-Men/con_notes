using System.Collections.Generic;
using System.Linq;
using System.Threading;
using App.Database;
using App.Presentation.Common;
using Cysharp.Threading.Tasks;
using Database.Impl;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace App.Presentation.Scenario
{
    public class ScenarioSelectViewRoot : MonoBehaviour
    {
        [SerializeField]
        private PopupDialogueView popupDialogueView;

        [SerializeField]
        private GameObject gateObject;

        [SerializeField]
        private List<ListItem> scenarioListItems;

        private List<string> scenarioTitles = new();

        private void Start()
        {
            Initialize();
            
            DOTween.Sequence()
                .Append(gateObject.transform.DOLocalRotate(Vector3.up * 180f, 5f).SetEase(Ease.Linear))
                .Append(gateObject.transform.DOLocalRotate(Vector3.up * 360f, 5f).SetEase(Ease.Linear))
                .SetLoops(-1, LoopType.Restart)
                .WithCancellation(gateObject.GetCancellationTokenOnDestroy());
        }

        private void Initialize()
        {
            var database = DatabaseFactory.ScenarioDatabase;
            scenarioTitles = database.All().Keys.ToList();
            
            // とりあえず表示できる分だけ表示する
            for (var i = 0; i < scenarioListItems.Count; i++)
            {
                var item = scenarioListItems[i];
                if (i >= scenarioTitles.Count)
                {
                    item.gameObject.SetActive(false);
                    continue;
                }
                var title = scenarioTitles[i];
                item.SetText(title);
                item.AsObservable().Subscribe(_ =>
                {
                    InitScenarioView(title).Forget();
                });
            }
            
        }
        
        private async UniTask InitScenarioView(string scenarioID)
        {
            var scenarioData = await GitHubScenarioFetcher.Instance.Fetch(scenarioID);
            if (scenarioData == null)
            {
                await ShowPopupDialogue($"このお話は読めないようです... ({scenarioID})");
                return;
            }

            await PageManager.PushAsyncWithFade("ScenarioScene", () =>
            {
                var scenarioViewRoot = PageManager.GetComponent<ScenarioViewRoot>();
                scenarioViewRoot.Initialize(new ScenarioViewRoot.ScenarioViewParam()
                {
                    scenarioData = scenarioData
                });
                return UniTask.CompletedTask;
            });
        }

        private async UniTask ShowPopupDialogue(string text)
        {
            await popupDialogueView.ShowPopup(text, CancellationToken.None, showButtons:false);
        }

        private async UniTask CloseScene()
        {
            await PageManager.PopAsync();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseScene().Forget();
            }
        }

        public void OnClickCloseButton()
        {
            CloseScene().Forget();
        }

        public void OnClickSongSelectButton()
        {
            PageManager.PushAsyncWithFade("SongSelectScene").Forget();
        }
    }
}