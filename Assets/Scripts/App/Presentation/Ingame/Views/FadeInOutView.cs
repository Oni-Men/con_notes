using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Ingame.Views
{
    public class FadeInOutView : MonoBehaviour
    {
        [SerializeField] private float waitDuration = 1f;

        [SerializeField] private bool fadeIn;

        [SerializeField] private float fadeInDuration = 1f;

        [SerializeField] private float stayDuration = 5f;

        [SerializeField] private bool fadeOut;

        [SerializeField] private float fadeOutDuration = 1f;

        [SerializeField] private Image panelImage;

        void Start()
        {
            // 待機時間待ってからフェードインを開始する
            Observable.Timer(TimeSpan.FromSeconds(waitDuration))
                .Subscribe(_ =>
                {
                    if (fadeIn)
                    {
                        PlayFadeIn();
                    }
                }).AddTo(this);

            if (stayDuration >= 0)
            {
                // 待機時間+フェードイン時間+滞在時間待ってからフェードアウトを開始する
                Observable.Timer(TimeSpan.FromSeconds(waitDuration + fadeInDuration + stayDuration))
                    .Subscribe(_ =>
                    {
                        if (fadeOut)
                        {
                            PlayFadeOut();
                        }
                    }).AddTo(this);
            }
        }

        public IObservable<long> PlayFadeIn()
        {
            panelImage.DOFade(0F, fadeInDuration);
            return Observable.Timer(TimeSpan.FromSeconds(fadeInDuration));
        }

        public IObservable<long> PlayFadeOut()
        {
            panelImage.DOFade(1F, fadeOutDuration);
            return Observable.Timer(TimeSpan.FromSeconds(fadeOutDuration));
        }
    }
}