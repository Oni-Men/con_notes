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
        [SerializeField] private TextMeshPro scoreText;

        [SerializeField] private TextMeshPro rankText;

        [SerializeField] private TextMeshPro maxComboText;

        [SerializeField] private Button retryButton;

        [SerializeField] private Button titleButton;

        void Start()
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

            scoreText.text = $"{score} ポイント";
            rankText.text = rank;
            maxComboText.text = $"{maxCombo} コンボ";
            
            retryButton.OnClickAsObservable().Subscribe(_ => ShowRetryScene()).AddTo(this);
            titleButton.OnClickAsObservable().Subscribe(_ => ShowTitleScene()).AddTo(this);
        }

        private void ShowRetryScene()
        {
            SceneManager.LoadScene("IngameScene");
        }

        private void ShowTitleScene()
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
}