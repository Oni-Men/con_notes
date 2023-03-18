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

namespace App.Presentation.Ingame.Presenters
{
    public class GamePresenter
    {
        private readonly InGameViewRoot inGameViewRoot;
        private readonly StatusViewRoot _statusViewRoot;
        private readonly NotePresenters _notePresenters;
        private readonly InputController _inputController;

        private GameModel _gameModel;
        public NotePresenters NotePresenters => _notePresenters;
        public string SongDirectoryPath { get; private set; }

        private ReactiveProperty<bool> _pause = new (false);
        public IObservable<bool> PauseState;

        public GamePresenter(InGameViewRoot inGameViewRoot, StatusViewRoot statusViewRoot,
            InputController inputController)
        {
            this.inGameViewRoot = inGameViewRoot;
            _statusViewRoot = statusViewRoot;
            _inputController = inputController;
            _notePresenters = new NotePresenters();
        }

        public void  Initialize(InGameViewRoot.InGameViewParam param)
        {
            _gameModel = GameManager.GetInstance().StartGame(this);
            _notePresenters.Initialize();

            SongDirectoryPath = param.songDirectoryPath;
            
            Bind();
        }

        private void Bind()
        {
            _gameModel.JudgeNotification.Subscribe(viewModel =>
            {
                ShowPopup(viewModel.LaneId, viewModel.JudgementType);
            });
            _gameModel.Score.Subscribe(score => _statusViewRoot.UpdateScore(score)).AddTo(_statusViewRoot);
            _gameModel.CurrentCombo.Subscribe(combo => _statusViewRoot.UpdateCombo(combo)).AddTo(_statusViewRoot);
            _gameModel.MaxCombo.Subscribe(maxCombo => _statusViewRoot.UpdateMaxCombo(maxCombo)).AddTo(_statusViewRoot);
            _gameModel.HealthLevel.Subscribe(health => _statusViewRoot.UpdateSlider(health)).AddTo(_statusViewRoot);

            _gameModel.GameEndEvent.Subscribe(HandleOnGameEnd).AddTo(inGameViewRoot);

            _inputController.LaneStateObserver.Subscribe(HandleInput).AddTo(_inputController);

            // 楽曲再生終了時に一回だけハンドラを実行する
            inGameViewRoot.EndPlayingEvent.Subscribe(_ => { _gameModel.FinalizeGame(); }).AddTo(inGameViewRoot);
            inGameViewRoot.TogglePauseEvent.Subscribe(_ => _pause.Value = !_pause.Value).AddTo(inGameViewRoot);
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
            
            if (laneState.IsPressed)
            {
                _gameModel.PressLane(laneState.LaneId);
                _gameModel.DoJudge(laneState.LaneId);
            }
            else
            {
                _gameModel.ReleaseLane(laneState.LaneId);
            }
        }

        public void UpdateComboCount(int combo)
        {
            _statusViewRoot.UpdateCombo(combo);
        }

        private void ShowPopup(int laneId, JudgementType type)
        {
            inGameViewRoot.SpawnFlyingText(laneId, type);
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

            inGameViewRoot.SpawnParticle(laneId, amount);
        }

        private async UniTask OnGameEnd(GameResultViewModel gameResultViewModel)
        {
            GameManager.GetInstance().AddResultViewModel(gameResultViewModel);

            var duration = 1.5f;
            var slowTask = _gameModel.IsAlive ? UniTask.CompletedTask : inGameViewRoot.PlaySlowEffect(duration);
            await slowTask;
            await PageManager.ReplaceAsync("ResultScene", () =>
            {
                PageManager.GetComponent<ResultRootView>()?.Initialize(gameResultViewModel);
            });
        }
    }
}