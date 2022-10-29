using System;
using System.Linq;
using System.Text;
using App.Application;
using App.Domain;
using App.Domain.Ingame.Enums;
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

        [SerializeField] private TMP_Text evalCountsText;

        [SerializeField] private Button retryButton;

        [SerializeField] private Button titleButton;

        [SerializeField] private AudioClip successBgm;
        [SerializeField] private AudioClip failBgm;
        
        void OnEnable()
        {
            var gameManager = GameManager.GetInstance();
            var resultViewModel = gameManager.GetResultList().LastOrDefault();
            
            if (resultViewModel == null)
            {
                throw new NullReferenceException("result is null.");
            }

            // 成功、失敗それぞれの場合に合わせてBGMを鳴らす
            AudioSource.PlayClipAtPoint(resultViewModel.IsSucceed ? successBgm : failBgm, Vector3.zero);

            // ボタンのハンドラを登録
            retryButton.OnClickAsObservable().Subscribe(_ => ShowRetryScene()).AddTo(this);
            titleButton.OnClickAsObservable().Subscribe(_ => ShowTitleScene()).AddTo(this);
            
            SetResultViewModel(resultViewModel);
        }

        private void SetResultViewModel(GameResultViewModel resultViewModel)
        {
            scoreText.text = $"{resultViewModel.Score} ポイント";
            rankText.text = resultViewModel.IsSucceed ? resultViewModel.RankText : "失敗...";
            maxComboText.text = $"{resultViewModel.MaxCombo} コンボ";

            evalCountsText.text
                = $"{GameConst.EvalNames[JudgementType.Perfect]}:   {resultViewModel.EvalCounts[JudgementType.Perfect]}\n" +
                  $"{GameConst.EvalNames[JudgementType.Good]}:  {resultViewModel.EvalCounts[JudgementType.Good]}\n" +
                  $"{GameConst.EvalNames[JudgementType.Bad]}:   {resultViewModel.EvalCounts[JudgementType.Bad]}\n" +
                  $"{GameConst.EvalNames[JudgementType.Miss]}:  {resultViewModel.EvalCounts[JudgementType.Miss]}";
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