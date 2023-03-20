using System.Threading;
using App.Domain.Scenario;
using App.Presentation.Ingame.Presenters;
using App.Presentation.Ingame.Views;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace App.Presentation.Scenario
{
    public class ScenarioPresenter
    {
        private ScenarioData scenarioData;
        private readonly ScenarioView _scenarioView;
        private GamePresenter gamePresenter;

        public ScenarioPresenter(ScenarioView view)
        {
            _scenarioView = view;
        }
        
        public void Init(ScenarioData scenarioData)
        {
            this.scenarioData = scenarioData;
            _scenarioView.ScenarioPlayCancelEvent.Subscribe(_ =>
            {
                OnScenarioCancelled().Forget();
            });
        }

        public async UniTask StartScenario()
        {
            var source = new CancellationTokenSource();
            
            await _scenarioView.ClearScene(source.Token);
            await _scenarioView.ShowTitle(scenarioData.title, source.Token);

            _scenarioView.JumpNextButton(source.Token).Forget();

            await _scenarioView.WaitForClickNextButton(source.Token);
            source.Cancel();
            await _scenarioView.FadeOutTitle();
            foreach (var page in scenarioData.pages) 
            {
                source = new CancellationTokenSource();
                
                await _scenarioView.ClearScene(source.Token);
                
                var showTextTask = _scenarioView.ShowText(page, source.Token);
                var waitForClickTask = _scenarioView.WaitForClickNextButton(source.Token);

                await UniTask.WhenAny(showTextTask, waitForClickTask);
                source.Cancel();
                source = new CancellationTokenSource();
                
                foreach (var command in page.commands)
                {
                    _scenarioView.JumpNextButton(source.Token).Forget();
                    await _scenarioView.WaitForClickNextButton(source.Token);
                    await ProcessCommand(command);
                }
                
                _scenarioView.JumpNextButton(source.Token).Forget();
                await _scenarioView.WaitForClickNextButton(source.Token);
                
                source.Cancel();
            }

            await OnScenarioEnd();
        }

        private async UniTask ProcessCommand(ScenarioCommand command)
        {
            switch (command.name)
            {
                case "play_song":
                    await PlaySong(command.args[0]);
                    break;
                default:
                    Debug.Log($"Unknown command: {command.name}");
                    break;
            }
        }

        private async UniTask PlaySong(string songPath)
        {
            await PageManager.PushAsyncWithFade("IngameScene", () => OnLoadInGame(songPath));
            var ingameViewRoot = PageManager.GetComponent<InGameViewRoot>();
            ingameViewRoot.StartGame();
        }

        private UniTask OnLoadInGame(string songPath)
        {
            var inGameViewRoot = PageManager.GetComponent<InGameViewRoot>();
            if (inGameViewRoot is null)
            {
                return UniTask.CompletedTask;
            }

            var param = new InGameViewRoot.InGameViewParam
            {
                songDirectoryPath = songPath
                // TODO スカイボックスを指定するパラメータを追加
            };
            inGameViewRoot.Initialize(param);

            return UniTask.CompletedTask;
        }

        private async UniTask OnScenarioCancelled()
        {
            Debug.Log("シナリオ再生キャンセル");
            await PageManager.PopAsync();
        }
        
        private async UniTask OnScenarioEnd()
        {
            Debug.Log("シナリオ終了");
            await PageManager.PopAsync();
        }

    }
}