using System;
using App.Application;
using App.Domain;
using App.Domain.Ingame.Enums;
using App.Presentation.Ingame.Views;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
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

        [SerializeField] private Button returnButton;

        [SerializeField] private AudioClip successBgm;
        [SerializeField] private AudioClip failBgm;

        [SerializeField]
        private string songDirectoryPath;
        
        public void Initialize(GameResultViewModel resultViewModel)
        {
            if (resultViewModel == null)
            {
                throw new NullReferenceException("result is null.");
            }
            // 成功、失敗それぞれの場合に合わせてBGMを鳴らす
            AudioSource.PlayClipAtPoint(resultViewModel.IsSucceed ? successBgm : failBgm, Vector3.zero);

            // ボタンのハンドラを登録
            retryButton.OnClickAsObservable().Subscribe(_ => OnClickRetryButton().Forget()).AddTo(this);
            returnButton.OnClickAsObservable().Subscribe(_ => OnReturnButtonClicked().Forget()).AddTo(this);
            
            SetResultViewModel(resultViewModel);
        }

        private void SetResultViewModel(GameResultViewModel resultViewModel)
        {
            songDirectoryPath = resultViewModel.SongDirectoryPath;
            
            scoreText.text = $"{resultViewModel.Score} ポイント";
            rankText.text = resultViewModel.IsSucceed ? resultViewModel.RankText : "失敗...";
            maxComboText.text = $"{resultViewModel.MaxCombo} コンボ";

            evalCountsText.text
                = $"{GameConst.EvalNames[JudgementType.Perfect]}:\t\t{resultViewModel.EvalCounts[JudgementType.Perfect]}\n" +
                  $"{GameConst.EvalNames[JudgementType.Good]}:\t\t{resultViewModel.EvalCounts[JudgementType.Good]}\n" +
                  $"{GameConst.EvalNames[JudgementType.Bad]}:\t\t{resultViewModel.EvalCounts[JudgementType.Bad]}\n" +
                  $"{GameConst.EvalNames[JudgementType.Miss]}:\t\t{resultViewModel.EvalCounts[JudgementType.Miss]}";
        } 

        private async UniTask OnClickRetryButton()
        {
            GameManager.ShouldPlayCutIn = false;
            await PageManager.PushAsyncWithFade("IngameScene", () =>
            {
                PageManager.GetComponent<InGameViewRoot>()?.Initialize(new InGameViewRoot.InGameViewParam()
                {
                    songDirectoryPath = songDirectoryPath
                });
            });
        }

        private async UniTask OnReturnButtonClicked()
        {
            await PageManager.PopAsync();
        }
    }
}