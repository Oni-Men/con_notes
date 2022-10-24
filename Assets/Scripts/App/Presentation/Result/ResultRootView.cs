using System;
using App.Domain;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace App.Presentation.Result
{
    public class ResultRootView : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;

        [SerializeField] private TMP_Text rankText;

        [SerializeField] private TMP_Text maxComboText;

        [SerializeField] private Button retryButton;

        [SerializeField] private Button titleButton;

        [SerializeField] private AudioClip successBgm;
        [SerializeField] private AudioClip failBgm;
        
        void OnEnable()
        {
            var gameManager = GameManager.GetInstance();
            var currentGame = gameManager.CurrentGame;
            if (currentGame == null)
            {
                throw new NullReferenceException("current game is null. we cannot refer result of the last game.");
            }

            var score = currentGame.Score.Value;
            var maxCombo = currentGame.MaxCombo.Value;
            var rank = currentGame.GetRank();

            if (currentGame.IsAlive && !rank.Equals("不可"))
            {
               AudioSource.PlayClipAtPoint(successBgm, Vector3.zero);
            }
            else
            {
                AudioSource.PlayClipAtPoint(failBgm, Vector3.zero);
            }
            
            scoreText.text = $"{score} ポイント";
            rankText.text = rank;
            maxComboText.text = $"{maxCombo} コンボ";
            
            retryButton.OnClickAsObservable().Subscribe(_ => ShowRetryScene()).AddTo(this);
            titleButton.OnClickAsObservable().Subscribe(_ => ShowTitleScene()).AddTo(this);
        }

        private void ShowRetryScene()
        {
            GameManager.ShouldPlayCutIn = false;
            SceneManager.LoadScene("IngameScene");
        }

        private void ShowTitleScene()
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
}