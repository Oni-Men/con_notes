using System.Threading;
using App.Domain.Scenario;
using Cysharp.Threading.Tasks;

namespace App.Presentation.Scenario
{
    public class ScenarioPresenter
    {
        private ScenarioRoot _scenarioRoot;
        private readonly ScenarioView _scenarioView;

        public ScenarioPresenter(ScenarioView view)
        {
            _scenarioView = view;
        }
        
        public void Init(ScenarioRoot scenarioRoot)
        {
            _scenarioRoot = scenarioRoot;
        }

        public async UniTask StartScenario()
        {
            var source = new CancellationTokenSource();
            
            await _scenarioView.ClearScene(source.Token);
            await _scenarioView.ShowTitle(_scenarioRoot.title, source.Token);

            _scenarioView.JumpNextButton(source.Token).Forget();

            await _scenarioView.WaitForClickNextButton(source.Token);
            source.Cancel();
            await _scenarioView.FadeOutTitle();
            foreach (var page in _scenarioRoot.pages) 
            {
                source = new CancellationTokenSource();
                await _scenarioView.ClearScene(source.Token);
                await _scenarioView.ShowText(page, source.Token);
                
                _scenarioView.JumpNextButton(source.Token).Forget();
                await _scenarioView.WaitForClickNextButton(source.Token);
                source.Cancel();
            }
        }

    }
}