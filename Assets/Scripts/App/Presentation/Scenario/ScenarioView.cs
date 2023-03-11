using System.Threading;
using App.Domain.Scenario;
using App.Presentation.Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Scenario
{
    public class ScenarioView : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;
        
        [SerializeField]
        private Button nextButton;

        [SerializeField]
        private PyonX2 nextButtonPyonx2;

        [SerializeField]
        private VerticalTextView verticalTextView;

        [SerializeField]
        private VerticalTextView verticalTitleView;

        private ScenarioPresenter _scenarioPresenter;

        public void Initialize(ScenarioRoot scenarioRoot)
        {
            _scenarioPresenter = new ScenarioPresenter(this);
            _scenarioPresenter.Init(scenarioRoot);
            _scenarioPresenter.StartScenario().Forget();
        }

        public async UniTask ClearScene(CancellationToken cancellationToken)
        {
            await DOTween.To(
                () => canvasGroup.alpha,
                value => canvasGroup.alpha = value,
                0.0f, 0.5f)
                .AwaitForComplete(TweenCancelBehaviour.Complete, cancellationToken);

            verticalTextView.ClearText();
            verticalTitleView.ClearText();
            canvasGroup.alpha = 1.0f;
        }

        private async UniTask SetSceneText(string text, CancellationToken cancellationToken)
        {
            canvasGroup.alpha = 1.0f;
            verticalTextView.ClearText();
            await verticalTextView.SetTextAsync(text, cancellationToken);
        }

        public async UniTask ShowTitle(string title, CancellationToken cancellationToken)
        {
            verticalTextView.ClearText();
            verticalTitleView.ClearText();
            await verticalTitleView.SetTextAsync(title, cancellationToken);
        }

        public async UniTask FadeOutTitle()
        {
            await verticalTitleView.FadeOut(1f);
        }

        public async UniTask JumpNextButton(CancellationToken token)
        {
            await nextButtonPyonx2.StartPyonX2(token);
        }
        
        public async UniTask ShowText(ScenarioPage scenarioPage, CancellationToken cancellationToken)
        {
            await SetSceneText(scenarioPage.text, cancellationToken);
        }

        public async UniTask WaitForClickNextButton(CancellationToken token)
        {
            var clickEvent = nextButton.onClick.GetAsyncEventHandler(token);
            await clickEvent.OnInvokeAsync();
        }
    }
}