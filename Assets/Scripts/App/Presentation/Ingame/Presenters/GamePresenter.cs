using System;
using App.Application;
using App.Common;
using App.Domain;
using App.Domain.Ingame;
using App.Domain.Ingame.Enums;
using App.Presentation.Ingame.Views;
using UniRx;
using UnityEngine.SceneManagement;

namespace App.Presentation.Ingame.Presenters
{
    public class GamePresenter
    {
        private readonly IngameViewRoot _ingameViewRoot;
        private readonly StatusViewRoot _statusViewRoot;
        private readonly NotePresenters _notePresenters;
        private readonly InputController _inputController;

        private GameModel _gameModel;
        public NotePresenters NotePresenters => _notePresenters;

        public GamePresenter(IngameViewRoot ingameViewRoot, StatusViewRoot statusViewRoot,
            InputController inputController)
        {
            _ingameViewRoot = ingameViewRoot;
            _statusViewRoot = statusViewRoot;
            _inputController = inputController;
            _notePresenters = new NotePresenters();
        }

        public void Initialize()
        {
            _gameModel = GameManager.GetInstance().StartGame(this);
            _notePresenters.Initialize();
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

            _gameModel.GameEndEvent.First().Subscribe(OnGameEnd);

            _inputController.LaneStateObserver.Subscribe(HandleInput).AddTo(_inputController);

            // 楽曲再生終了時に一回だけハンドラを実行する
            _ingameViewRoot.EndPlayingEvent
                .First()
                .Subscribe(_ => { _gameModel.FinalizeGame(); });
        }

        private void HandleInput(InputController.LaneStateData laneState)
        {
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
            _ingameViewRoot.SpawnFlyingText(laneId, type);
        }

        public void SpawnParticle(int laneId, JudgementType type)
        {
            var amount = 0f;
            switch (type)
            {
                case JudgementType.Perfect:
                    amount = 1f;
                    break;
                case JudgementType.Good:
                    amount = 0.5f;
                    break;
                case JudgementType.Bad:
                    amount = 0.25f;
                    break;
            }

            _ingameViewRoot.SpawnParticle(laneId, amount);
        }

        private void OnGameEnd(GameResultViewModel gameResultViewModel)
        {
            GameManager.GetInstance().AddResultViewModel(gameResultViewModel);

            IObservable<long> timer;
            if (!_gameModel.IsAlive)
            {
                _ingameViewRoot.PlaySlowEffect();
            }

            _ingameViewRoot.fadeInoutView.PlayFadeOut().Subscribe(_ =>
            {
                // リザルトシーンへ遷移
                SceneManager.LoadScene("ResultScene");
            });
        }
    }
}