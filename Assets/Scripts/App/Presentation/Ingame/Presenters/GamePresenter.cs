using App.Application.Ingame;
using App.Common;
using App.Domain;
using App.Domain.Ingame;
using App.Domain.Ingame.Enums;
using App.Presentation.Ingame.Views;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace App.Presentation.Ingame.Presenters
{
    public class GamePresenter
    {
        private readonly DebugView _debugView;
        private readonly IngameViewRoot _ingameViewRoot;
        private readonly StatusViewRoot _statusViewRoot;
        private readonly NotePresenters _notePresenters;
        private readonly InputController _inputController;

        private GameModel _gameModel;
        public NotePresenters NotePresenters => _notePresenters;

        public GamePresenter(IngameViewRoot ingameViewRoot, StatusViewRoot statusViewRoot,
            InputController inputController, DebugView debugView)
        {
            _ingameViewRoot = ingameViewRoot;
            _debugView = debugView;
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

            _inputController.LaneStateObserver.Subscribe(laneState =>
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
            }).AddTo(_inputController);
            
            _ingameViewRoot.EndPlayingEvent.Subscribe(_ =>
            {
                //TODO ゲーム終了処理
                Debug.Log("end playing event");
            });
            
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
    }
}