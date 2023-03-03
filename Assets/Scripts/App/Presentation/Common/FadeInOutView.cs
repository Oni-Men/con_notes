using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Common
{
    public class FadeInOutView : MonoBehaviour
    {
        [SerializeField]
        private bool playOnAwake = false;

        [SerializeField]
        private float waitDuration = 1f;

        [SerializeField]
        private float fadeInDuration = 1f;

        [SerializeField]
        private float stayDuration = 5f;

        [SerializeField]
        private float fadeOutDuration = 1f;

        [SerializeField]
        private Image panelImage;

        private async void Start()
        {
            if (!playOnAwake)
            {
                return;
            }

            var ct = gameObject.GetCancellationTokenOnDestroy();
            await UniTask.Delay(TimeSpan.FromSeconds(waitDuration), cancellationToken: ct);
            await PlayFadeIn(ct);

            if (stayDuration <= 0)
            {
                return;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(stayDuration), cancellationToken: ct);
            await PlayFadeOut(ct);
        }

        public async UniTask PlayFadeIn(CancellationToken ct)
        {
            await panelImage
                .DOFade(0F, fadeInDuration)
                .WithCancellation(ct);
        }

        public async UniTask PlayFadeOut(CancellationToken ct)
        {
            await panelImage.DOFade(1F, fadeOutDuration).WithCancellation(ct);
        }
    }
}