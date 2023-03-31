using System;
using App.Application;
using App.Domain;
using App.Domain.Ingame.Enums;
using App.Presentation.Ingame.Views;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Result
{
    public class ResultRootView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text scoreText;

        [SerializeField]
        private TMP_Text rankText;

        [SerializeField]
        private TMP_Text maxComboText;

        [SerializeField]
        private TMP_Text evalCountsText;

        [SerializeField]
        private Button retryButton;

        [SerializeField]
        private Button returnButton;

        [SerializeField]
        private AudioClip successBgm;

        [SerializeField]
        private AudioClip failBgm;

        [SerializeField]
        private string songDirectoryPath;

        public void Initialize(GameResultViewModel resultViewModel)
        {
            if (resultViewModel == null)
            {
                throw new NullReferenceException("result is null.");
            }

            // 成功、失敗それぞれの場合に合わせてBGMを鳴らす
            AudioSource.PlayClipAtPoint(resultViewModel.isSucceed ? successBgm : failBgm, Vector3.zero);

            // ボタンのハンドラを登録
            retryButton.OnClickAsObservable().Subscribe(_ => OnClickRetryButton().Forget()).AddTo(this);
            returnButton.OnClickAsObservable().Subscribe(_ => OnReturnButtonClicked().Forget()).AddTo(this);

            SetResultViewModel(resultViewModel);
        }

        private void SetResultViewModel(GameResultViewModel resultViewModel)
        {
            songDirectoryPath = resultViewModel.songDirectoryPath;

            rankText.text = resultViewModel.isSucceed ? resultViewModel.rankText : "失敗...";
            scoreText.text = $"{resultViewModel.score} ポイント";
            maxComboText.text = $"{resultViewModel.maxCombo} コンボ";

            evalCountsText.text
                = $"{GameConst.EvalNames[JudgementType.Perfect]}:\t\t{resultViewModel.evalCounts[JudgementType.Perfect]}\n" +
                  $"{GameConst.EvalNames[JudgementType.Good]}:\t\t{resultViewModel.evalCounts[JudgementType.Good]}\n" +
                  $"{GameConst.EvalNames[JudgementType.Bad]}:\t\t{resultViewModel.evalCounts[JudgementType.Bad]}\n" +
                  $"{GameConst.EvalNames[JudgementType.Miss]}:\t\t{resultViewModel.evalCounts[JudgementType.Miss]}";
            
            PlayAnimation();
        }

        public void PlayAnimation()
        {
            const float moveAmount = 500f;
            
            rankText.transform.Translate(Vector3.down * moveAmount);
            scoreText.transform.Translate(Vector3.down * moveAmount);
            maxComboText.transform.Translate(Vector3.down * moveAmount);

            rankText.alpha = 0f;
            scoreText.alpha = 0f;
            maxComboText.alpha = 0f;
            evalCountsText.alpha = 0f;
            
            const float duration = 0.75f;
            const float interval = 0.1f;

            var seq1 = DOTween.Sequence()
                .Append(rankText.transform.DOLocalMoveY(moveAmount, duration).SetRelative().SetEase(Ease.OutCirc))
                .Join(rankText.DOFade(1f, duration))
                .SetDelay(interval);
            
            var seq2 = DOTween.Sequence()
                .Append(scoreText.transform.DOLocalMoveY(moveAmount, duration).SetRelative().SetEase(Ease.OutCirc))
                .Join(scoreText.DOFade(1f, duration))
                .SetDelay(interval * 2);
            
            var seq3 = DOTween.Sequence()
                .Append(maxComboText.transform.DOLocalMoveY(moveAmount, duration).SetRelative().SetEase(Ease.OutCirc))
                .Join(maxComboText.DOFade(1f, duration))
                .SetDelay(interval * 3);

            DOTween.Sequence()
                .Join(seq1)
                .Join(seq2)
                .Join(seq3)
                .AppendInterval(interval)
                .Join(evalCountsText.DOFade(1f, duration));
        }

        private async UniTask OnClickRetryButton()
        {
            await PageManager.ReplaceAsync("IngameScene", () =>
            {
                PageManager.GetComponent<InGameViewRoot>()?.Initialize(new InGameViewRoot.InGameViewParam()
                {
                    songDirectoryPath = songDirectoryPath
                });
                return UniTask.CompletedTask;
            });

            var ingameViewRoot = PageManager.GetComponent<InGameViewRoot>();
            ingameViewRoot.StartGame();
        }

        private async UniTask OnReturnButtonClicked()
        {
            await PageManager.PopAsync();
        }

        public void OnClickCloseButton()
        {
            OnReturnButtonClicked().Forget();
        }
    }
}