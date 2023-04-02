using System;
using System.Threading;
using App.Presentation.Scenario;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Common
{
    public class PopupDialogueView : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private Image container;

        [SerializeField]
        private TMP_Text message;

        [SerializeField]
        private Button buttonOk;

        [SerializeField]
        private Button buttonNg;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public async UniTask<bool> ShowPopup(string text, CancellationToken cancellationToken, bool showButtons = true)
        {
            canvasGroup.alpha = 0f;
            container.transform.localScale = Vector3.one * 0.75f;
            gameObject.SetActive(true);

            buttonOk.gameObject.SetActive(showButtons);
            buttonNg.gameObject.SetActive(showButtons);
            message.SetText(text);

            await DOTween.Sequence()
                .Join(canvasGroup.DOFade(1.0f, 0.3f))
                .Join(container.transform.DOScale(1.0f, 0.3f).SetEase((Ease.OutBack)));


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
            
            await DOTween.Sequence()
                .Join(canvasGroup.DOFade(0.0f, 0.3f))
                .Join(container.transform.DOScale(0.75f, 0.3f).SetEase((Ease.OutBack)));

            gameObject.SetActive(false);
            return ok;
        }
    }
}