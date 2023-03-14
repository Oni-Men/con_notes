using System.Linq;
using System.Threading;
using App.Domain.Scenario;
using App.Presentation.Ingame.Views;
using Cysharp.Threading.Tasks;
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

                _scenarioView.JumpNextButton(source.Token).Forget();
                await _scenarioView.WaitForClickNextButton(source.Token);
                
                foreach (var command in page.commands)
                {
                    await ProcessCommand(command);
                }
                
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
            await SceneManager.LoadSceneAsync("IngameScene", LoadSceneMode.Additive);
            var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            var inGameViewRoot = rootGameObjects
                .Select(go => go.GetComponent<InGameViewRoot>())
                .FirstOrDefault(view => view is not null);

            if (inGameViewRoot is null)
            {
                return;
            }

            var param = new InGameViewRoot.InGameViewParam
            {
                songDirectoryPath = songPath
            };
            await inGameViewRoot.Initialize(param);
        }

        private async UniTask OnScenarioEnd()
        {
            Debug.Log("シナリオ終了");
            Debug.Log(scenarioData.songPath);

            if (string.IsNullOrEmpty(scenarioData.songPath))
            {
                return;
            }

            SceneManager.LoadScene("ScenarioScene");
            var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            var ingameViewRoot = rootGameObjects
                .Select(go => go.GetComponent<InGameViewRoot>())
                .FirstOrDefault(view => view is not null);

            if (ingameViewRoot is null)
            {
                return;
            }

            var param = new InGameViewRoot.InGameViewParam{};
            await ingameViewRoot.Initialize(param);
        }

    }
}