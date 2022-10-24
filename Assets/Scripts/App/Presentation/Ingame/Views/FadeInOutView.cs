using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

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

    public void PlayFadeIn()
    {
        if (panelImage == null)
        {
            return;
        }

        panelImage.DOFade(0F, fadeInDuration);
    }

    public void PlayFadeOut()
    {
        if (panelImage == null)
        {
            return;
        }

        panelImage.DOFade(1F, fadeOutDuration);
    }
}