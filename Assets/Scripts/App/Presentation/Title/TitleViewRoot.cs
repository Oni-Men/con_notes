using System;
using App.Domain;
using App.Presentation.Ingame.Views;
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

        [SerializeField] private FadeInOutView fader;

        void Awake()
        {
            GameConst.LoadMasterData();
            
            startButton.OnClickAsObservable().Subscribe(_ => ShowIngameScene()).AddTo(this);
            exitButton.OnClickAsObservable().Subscribe(_ => ExitGame()).AddTo(this);

            GameManager.ShouldPlayCutIn = true;
        }

        private void ShowIngameScene()
        {
            fader.PlayFadeOut()
                .Subscribe(_ => { SceneManager.LoadScene("IngameScene"); });
        }

        private void ExitGame()
        {
            //TODO Impl exit process.
            UnityEngine.Application.Quit();
        }
    }
}