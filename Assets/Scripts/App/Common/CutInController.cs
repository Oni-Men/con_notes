using System;
using App.Domain;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace App.Common
{
    public class CutInController : MonoBehaviour
    {
        [SerializeField] private GameObject videoObject;

        [SerializeField] private float waitSeconds;

        [SerializeField] private float fadeTime;

        private VideoPlayer _videoPlayer;
        private RawImage _rawImage;
    
        void Awake()
        {
            _videoPlayer = videoObject.GetComponent<VideoPlayer>();
            _rawImage = videoObject.GetComponent<RawImage>();
        }

        void Start()
        {
            videoObject.gameObject.SetActive(false);
            _videoPlayer.playOnAwake = false;
        
            if (GameManager.ShouldPlayCutIn)
            {
                PlayCutInVideo();
            }
        }

        private void PlayCutInVideo()
        {
            // 待機時間経過後にアクティブ化 & 再生
            Observable.Timer(TimeSpan.FromSeconds(waitSeconds)).Subscribe(_ =>
            {
                videoObject.gameObject.SetActive(true);
                _videoPlayer.Play();
            
                _rawImage.canvasRenderer.SetAlpha(1f);
            }).AddTo(this);

            // 再生終了前に透明フェードする
            Observable.EveryUpdate()
                .Where(_ => _videoPlayer.time + fadeTime > _videoPlayer.length)
                .First()
                .Subscribe(_ =>
                {
                    _rawImage.DOFade(0f, fadeTime);
                }).AddTo(this);

            // 動画再生終了時に非アクティブ化
            _videoPlayer.loopPointReached += src => { src.gameObject.SetActive(false); };
        }
    }
}