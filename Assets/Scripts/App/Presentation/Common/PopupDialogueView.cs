using System;
using System.Threading;
using App.Presentation.Scenario;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Common
{
    public class PopupDialogueView : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private VerticalTextView verticalTextView;

        [SerializeField]
        private Button buttonOk;

        [SerializeField]
        private Button buttonNg;

        private void Start()
        {
            canvasGroup.alpha = 0f;
        }


        public async UniTask<bool> ShowPopup(string text, CancellationToken cancellationToken, bool showButtons = true)
        {
            buttonOk.gameObject.SetActive(showButtons);
            buttonNg.gameObject.SetActive(showButtons);
            await canvasGroup.DOFade(1.0f, 0.5f).WithCancellation(cancellationToken);
            await verticalTextView.SetTextAsync(text, cancellationToken);

            var ok = false;
            var clicked = false;
            buttonOk.OnClickAsObservable().Subscribe(_ =>
            {
                ok = true;
                clicked = true;
            }).AddTo(cancellationToken);
            buttonNg.OnClickAsObservable().Subscribe(_ => { clicked = true; }).AddTo(cancellationToken);

            if (showButtons)
            {
                await UniTask.WaitUntil(() => clicked, cancellationToken: cancellationToken);
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromSeconds(5), cancellationToken: cancellationToken);
            }
            return ok;
        }
    }
}