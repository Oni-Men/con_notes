using System;
using App.Domain;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Video;

public class CutInController : MonoBehaviour
{

    [SerializeField] private VideoPlayer videoObject;

    [SerializeField] private float waitSeconds;

    [SerializeField] private float videoTime;
    
    void Awake(){
        // As initialization, make a video disable.
        videoObject.gameObject.SetActive(false);
        
        if (GameManager.ShouldPlayCutIn)
        {
            PlayCutInVideo();
        }
    }

    private void PlayCutInVideo()
    {
        Observable.Timer(TimeSpan.FromSeconds(waitSeconds)).Subscribe(_ =>
        {
            videoObject.gameObject.SetActive(true);
            Observable.Timer(TimeSpan.FromSeconds(videoTime)).Subscribe(_ =>
            {
                videoObject.Stop();
                videoObject.gameObject.SetActive(false);
            });
        });
    }
}
