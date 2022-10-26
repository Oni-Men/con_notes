using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace App.Presentation.Title
{
    public class TitleViewRoot : MonoBehaviour
    {
        [SerializeField] private Button startButton;

        [SerializeField] private Button exitButton;

        [SerializeField] private GameObject clickEffect;

        [SerializeField] private FadeInOutView fader;

        void Awake()
        {
            startButton.OnClickAsObservable().Subscribe(_ => ShowIngameScene()).AddTo(this);
            exitButton.OnClickAsObservable().Subscribe(_ => ExitGame()).AddTo(this);
        }

        private void ShowIngameScene()
        {
            
            fader.PlayFadeOut();
            
            UniTask.Delay(TimeSpan.FromSeconds(1)).ToObservable().Subscribe(_ =>
            {
                SceneManager.LoadScene("IngameScene");
            });
        }

        private void ExitGame()
        {
            //TODO Impl exit process.
            UnityEngine.Application.Quit();
        }
    }
}