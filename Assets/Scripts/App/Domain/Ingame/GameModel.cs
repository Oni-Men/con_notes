using System;
using System.Collections.Generic;
using App.Application;
using App.Application.Ingame;
using App.Domain.Ingame.Enums;
using App.Presentation.Ingame.Presenters;
using UniRx;
using UnityEngine;

namespace App.Domain.Ingame
{
    public class GameModel
    {
        private bool end = false;
        
        private Dictionary<JudgementType, int> _evalCounts;
        private readonly GamePresenter _presenter;
        private readonly ReactiveProperty<int> _score = new();
        private readonly ReactiveProperty<int> _currentCombo = new();
        private readonly ReactiveProperty<int> _maxCombo = new();
        private readonly ReactiveProperty<int> _healthLevel = new();
        private Dictionary<int, LaneState> _laneStates;

        public string SongDirectoryPath => _presenter.SongDirectoryPath;
        
        public GamePresenter Presenter => _presenter;
        public IReadOnlyDictionary<JudgementType, int> EvalCounts => _evalCounts;
        public IReadOnlyReactiveProperty<int> Score => _score;
        public IReadOnlyReactiveProperty<int> CurrentCombo => _currentCombo;
        public IReadOnlyReactiveProperty<int> MaxCombo => _maxCombo;
        public readonly Subject<JudgementViewModel> JudgeNotification = new();
        public  IReadOnlyReactiveProperty<int> HealthLevel => _healthLevel;
        public bool IsAlive => _healthLevel.Value > 0;
        private readonly Subject<GameResultViewModel> _gameEndEvent = new();
        public IObservable<GameResultViewModel> GameEndEvent => _gameEndEvent.First().AsObservable();
        
        public GameModel(GamePresenter presenter)
        {
            _presenter = presenter;
        }

        public void Initialize()
        {
            _evalCounts = new Dictionary<JudgementType, int>()
            {
                { JudgementType.Perfect , 0},
                { JudgementType.Good , 0},
                { JudgementType.Bad, 0},
                { JudgementType.Miss , 0},
            };
            _score.Value = 0;
            _currentCombo.Value = 0;
            _maxCombo.Value = 0;
            _healthLevel.Value = 100;
            _laneStates = new Dictionary<int, LaneState>()
            {
                { -1, new LaneState() },
                { 0, new LaneState() },
                { 1, new LaneState() },
            };
        }

        public void FinalizeGame()
        {
            _maxCombo.Value = Math.Max(_maxCombo.Value, _currentCombo.Value);
            _gameEndEvent.OnNext(new GameResultViewModel(this));
            end = true;
        }

        public void PressLane(int laneId)
        {
            if (!_laneStates.ContainsKey(laneId))
            {
                throw new ArgumentException("No such laneId");
            }

            _laneStates[laneId].Press();
        }

        public void ReleaseLane(int laneId)
        {
            if (!_laneStates.ContainsKey(laneId))
            {
                throw new ArgumentException("No such laneId");
            }

            _laneStates[laneId].Release();
        }

        public void DoJudge(int laneId)
        {
            if (end) return;
            
            var nearestNotePresenter = _presenter.NotePresenters.GetNearestNotePresenter(laneId);
            if (nearestNotePresenter == null)
            {
                return;
            }

            var distance = Math.Abs(nearestNotePresenter.ZPosition);
            if (distance > GameConst.EvalAndThresholds[JudgementType.Bad] * 10)
            {
                return;
            }
            
            var judgementType = nearestNotePresenter.Judge(distance);
            AddJudgement(judgementType);
            
            //Debug.Log(judgementType);

            if (judgementType != JudgementType.Miss)
            {
                _presenter.SpawnParticle(laneId, judgementType);
            }
            
            if (!IsAlive)
            {
                FinalizeGame();
            }

            JudgeNotification.OnNext(new JudgementViewModel(laneId, judgementType));
        }

        public void ProcessPassedNote(NotePresenter presenter)
        {
            if (end) return;
            AddJudgement(JudgementType.Miss);
            _presenter.SpawnParticle(presenter.LaneId, JudgementType.Miss);
            JudgeNotification.OnNext(new JudgementViewModel(presenter.LaneId, JudgementType.Miss));
        }
        
        private void AddJudgement(JudgementType type)
        {
            if (!IsAlive || end) 
                return;
            
            UpdateHealthLevel(type);
            //判定リストに判定を追加する
            _evalCounts[type]++;

            //ミスのときコンボをリセット、そうでないときコンボを加算
            if (type == JudgementType.Miss)
            {
                _maxCombo.Value = Math.Max(_maxCombo.Value, _currentCombo.Value);
                _currentCombo.Value = 0;
            }
            else
            {
                _currentCombo.Value++;
            }

            //判定に応じてスコアを加算する
            var point = GetPointForEvaluation(type);
            var bonus = CalcBonusAmount(point);
            _score.Value += point + bonus;

            _presenter.UpdateComboCount(_currentCombo.Value);
        }

        private int CalcBonusAmount(int value)
        {
            var scale = (int)Mathf.Log(_currentCombo.Value, 5);
            return value * scale;
        }

        private static int GetPointForEvaluation(JudgementType type)
        {
            if (!GameConst.EvalAndPoints.ContainsKey(type))
            {
                throw new KeyNotFoundException();
            }
            return GameConst.EvalAndPoints[type];
        }

        private void UpdateHealthLevel(JudgementType type)
        {
            if (!IsAlive || end)
            {
                return;
            }

            var health = _healthLevel.Value;

            switch (type)
            {
                case JudgementType.Perfect:
                    health += 10;
                    break;
                case JudgementType.Good:
                    health += 5;
                    break;
                case JudgementType.Bad:
                    health += 1;
                    break;
                case JudgementType.Miss:
                    health -= 10;
                    break;
                case JudgementType.NotJudged:
                    break;
            }

            _healthLevel.Value = Mathf.Clamp(health, 0, 100);
        }

    }
}