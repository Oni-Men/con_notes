using System;
using App.Application;
using App.Common;
using App.Domain;
using App.Domain.Ingame;
using App.Domain.Ingame.Enums;
using App.Presentation.Ingame.Views;
using App.Presentation.Result;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace App.Presentation.Ingame.Presenters
{
    public class GamePresenter : IDisposable
    {
        private readonly InGameViewRoot _inGameViewRoot;
        private readonly StatusViewRoot _statusViewRoot;
        private readonly InputController _inputController;
        private readonly NoteViewPool _noteViewPool;

        private GameData gameData;
        public NotePresenters NotePresenters { get; }

        public string SongDirectoryPath { get; private set; }

        private ReactiveProperty<bool> _pause = new(false);
        public IObservable<bool> PauseState;

        public bool IsResultPageDisposed { get; private set; }

        public GamePresenter(InGameViewRoot inGameViewRoot, StatusViewRoot statusViewRoot,
            InputController inputController, NoteViewPool noteViewPool)
        {
            _inGameViewRoot = inGameViewRoot;
            _statusViewRoot = statusViewRoot;
            _inputController = inputController;
            _noteViewPool = noteViewPool;
            NotePresenters = new NotePresenters();
        }

        public void Initialize(InGameViewRoot.InGameViewParam param)
        {
            gameData = new GameData();
            gameData.Initialize(this);

            SongDirectoryPath = param.songDirectoryPath;

            Bind();
        }

        private void Bind()
        {
            gameData.judgeNotification.Subscribe(viewModel =>
            {
                ShowPopup(viewModel.LaneId, viewModel.JudgementType);
            });
            gameData.Score.Subscribe(score => _statusViewRoot.UpdateScore(score)).AddTo(_statusViewRoot);
            gameData.CurrentCombo.Subscribe(combo => _statusViewRoot.UpdateCombo(combo)).AddTo(_statusViewRoot);
            gameData.HealthLevel.Subscribe(health => _statusViewRoot.UpdateSlider(health)).AddTo(_statusViewRoot);

            gameData.GameEndEvent.Subscribe(HandleOnGameEnd).AddTo(_inGameViewRoot);

            _inputController.LaneState.Subscribe(HandleInput).AddTo(_inputController);

            // 楽曲再生終了時に一回だけハンドラを実行する
            _inGameViewRoot.EndPlayingEvent.Subscribe(_ => { gameData.FinalizeGame(); }).AddTo(_inGameViewRoot);
            _inGameViewRoot.TogglePauseEvent.Subscribe(_ => _pause.Value = !_pause.Value).AddTo(_inGameViewRoot);
        }

        private async void HandleOnGameEnd(GameResultViewModel result)
        {
            await OnGameEnd(result);
        }

        private void HandleInput(InputController.LaneStateData laneState)
        {
            if (_pause.Value)
            {
                return;
            }
            if (laneState.isPressed)
            {
                gameData.DoJudge(laneState.laneId);
            }
        }

        public void UpdateComboCount(int combo)
        {
            _statusViewRoot.UpdateCombo(combo);
        }

        public void SpawnNote(int laneId)
        {
            var noteView = _noteViewPool.GetNoteView();
            var notePresenter = new NotePresenter();
            notePresenter.Initialize(this, NotePresenters, noteView, laneId);
        }

        public void DespawnNote(NoteView noteView)
        {
            _noteViewPool.ReleaseNoteView(noteView);
        }

        private void ShowPopup(int laneId, JudgementType type)
        {
            if (type != JudgementType.Miss)
            {
                SpawnParticle(laneId, type);
            }
            _inGameViewRoot.SpawnFlyingText(laneId, type);
        }

        public void SpawnParticle(int laneId, JudgementType type)
        {
            var amount = type switch
            {
                JudgementType.Perfect => 1f,
                JudgementType.Good => 0.5f,
                JudgementType.Bad => 0.25f,
                _ => 0f
            };

            _inGameViewRoot.SpawnParticle(laneId, amount);
        }

        private async UniTask OnGameEnd(GameResultViewModel gameResultViewModel)
        {
            GameManager.GetInstance().AddResultViewModel(gameResultViewModel);

            const float duration = 1.5f;
            var slowTask = gameData.IsAlive ? UniTask.CompletedTask : _inGameViewRoot.PlaySlowEffect(duration);
            await slowTask;
            await PageManager.ReplaceAsync("ResultScene", () => OnLoadResult(gameResultViewModel));

            var resultRootView = PageManager.GetComponent<ResultRootView>();
            try
            {
                await UniTask.Never(resultRootView.gameObject.GetCancellationTokenOnDestroy());
            }
            catch (OperationCanceledException _)
            {
            }

            IsResultPageDisposed = true;
        }

        private UniTask OnLoadResult(GameResultViewModel gameResultViewModel)
        {
            PageManager.GetComponent<ResultRootView>()?.Initialize(gameResultViewModel); 
            return UniTask.CompletedTask;
        }

        public void OnNotePassed(NotePresenter notePresenter)
        {
            gameData.ProcessPassedNote(notePresenter);
        }

        public void Dispose()
        {
        }
    }
}