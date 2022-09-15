using App.Application.Ingame;
using App.Common;
using App.Domain;
using App.Domain.Ingame;
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
        
        public GamePresenter(IngameViewRoot ingameViewRoot, StatusViewRoot statusViewRoot, InputController inputController, DebugView debugView)
        {
            _ingameViewRoot = ingameViewRoot;
            _debugView = debugView;
            _statusViewRoot = statusViewRoot;
            _inputController = inputController;
            _notePresenters = new NotePresenters();
        }

        public void Initialize()
        {
            _gameModel =  GameManager.GetInstance().StartGame(this);
            _notePresenters.Initialize();
            Bind();
        }

        private void Bind()
        {
            _gameModel.JudgeNotification.Subscribe(ShowJudgementAsync);
            _gameModel.Score.Subscribe(score => _statusViewRoot.UpdateScore(score)).AddTo(_statusViewRoot);
            _gameModel.CurrentCombo.Subscribe(combo => _statusViewRoot.UpdateCombo(combo)).AddTo(_statusViewRoot);
            _gameModel.MaxCombo.Subscribe(maxCombo => _statusViewRoot.UpdateMaxCombo(maxCombo)).AddTo(_statusViewRoot);
        }

        public void UpdateComboCount(int combo)
        {
            _statusViewRoot.UpdateCombo(combo);
        }

        private void ShowJudgementAsync(JudgementViewModel judgementViewModel)
        {
            var judgementView = _ingameViewRoot.CreateJudgementView(judgementViewModel);
            judgementView.ShowJudgementAsync(judgementViewModel.JudgementType).Forget();
        }

        public void SpawnParticle(int laneId)
        {
            _ingameViewRoot.SpawnParticle(laneId);
        }
    }
}