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

        [SerializeField]
        private Button closeButton;
        
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
            closeButton.gameObject.SetActive(!showButtons);
            
            message.SetText(text);

            await DOTween.Sequence()
                .Join(canvasGroup.DOFade(1.0f, 0.3f))
                .Join(container.transform.DOScale(1.0f, 0.3f).SetEase((Ease.OutBack)));


            var ok = false;
            var clicked = false;
            if (showButtons)
            {
                buttonOk.OnClickAsObservable().Subscribe(_ =>
                {
                    ok = true;
                    clicked = true;
                }).AddTo(cancellationToken);
                buttonNg.OnClickAsObservable().Subscribe(_ => { clicked = true; }).AddTo(cancellationToken);
                await UniTask.WaitUntil(() => clicked, cancellationToken: cancellationToken);
            }
            else
            {
                closeButton.OnClickAsObservable().Subscribe(_ => clicked = true).AddTo(cancellationToken);
                await UniTask.WaitUntil(() => clicked, cancellationToken: cancellationToken);
            }
            
            await DOTween.Sequence()
                .Join(canvasGroup.DOFade(0.0f, 0.3f))
                .Join(container.transform.DOScale(0.75f, 0.3f).SetEase((Ease.InBack)));

            gameObject.SetActive(false);
            return ok;
        }
    }
}