using System;
using System.Threading;
using App.Domain;
using App.Presentation.Common;
using App.Presentation.Ingame.Views;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace App.Presentation.Title
{
    public class TitleViewRoot : MonoBehaviour
    {
        [SerializeField]
        private Button startButton;

        [SerializeField]
        private Button exitButton;

        [SerializeField]
        private FadeInOutView fader;

        void Awake()
        {
            GameConst.LoadMasterData();

            startButton.OnClickAsObservable().Subscribe(_ => ShowIngameScene().Forget()).AddTo(this);
            exitButton.OnClickAsObservable().Subscribe(_ => ExitGame().Forget()).AddTo(this);

            GameManager.ShouldPlayCutIn = true;
        }

        private async UniTask ShowIngameScene()
        {
            await fader.PlayFadeOut(CancellationToken.None);
            SceneManager.LoadScene("IngameScene");
        }

        private async UniTask ExitGame()
        {
            await fader.PlayFadeOut(CancellationToken.None);
            UnityEngine.Application.Quit();
        }
    }
}