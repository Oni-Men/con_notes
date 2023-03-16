using System.Linq;
using System.Threading;
using App.Domain.Scenario;
using App.Presentation.Ingame.Views;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Presentation.Scenario
{
    public class ScenarioPresenter
    {
        private ScenarioData scenarioData;
        private readonly ScenarioView _scenarioView;

        public ScenarioPresenter(ScenarioView view)
        {
            _scenarioView = view;
        }
        
        public void Init(ScenarioData scenarioData)
        {
            this.scenarioData = scenarioData;
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
                await _scenarioView.ShowText(page, source.Token);

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
            await PageManager.PushAsync("IngameScene");
            var inGameViewRoot = PageManager.GetComponent<InGameViewRoot>();
            if (inGameViewRoot is null)
            {
                return;
            }

            var param = new InGameViewRoot.InGameViewParam
            {
                songDirectoryPath = songPath
            };
            await inGameViewRoot.Initialize(param);
            // await UniTask.WaitUntil(() => inGameViewRoot.IsDestroyed());

        }

        private async UniTask OnScenarioEnd()
        {
            Debug.Log("シナリオ終了");
            Debug.Log(scenarioData.songPath);

            await PageManager.PopAsync();

        }

    }
}