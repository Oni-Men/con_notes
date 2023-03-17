using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Common
{
    public class FadeInOutView : MonoBehaviour
    {

        [SerializeField]
        private float fadeInDuration = 1f;

        [SerializeField]
        private float fadeOutDuration = 1f;

        [SerializeField]
        private Image panelImage;

        public void SetPanelBlack()
        {
        }

        public void SetPanelTransparent()
        {
            panelImage.color = Color.clear;
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