using System;
using System.Collections.Generic;
using App.Application.Ingame;
using App.Domain.Ingame.Enums;
using App.Presentation.Ingame.Presenters;
using UniRx;

namespace App.Domain.Ingame
{
    public class GameModel
    {
        private Beatmap _beatmap;

        private Dictionary<JudgementType, int> _judgeList;

        private readonly GamePresenter _presenter;

        private readonly ReactiveProperty<int> _score = new();

        private readonly ReactiveProperty<int> _currentCombo = new();

        private readonly ReactiveProperty<int> _maxCombo = new();


        public GamePresenter Presenter => _presenter;

        public IReadOnlyDictionary<JudgementType, int> JudgementList => _judgeList;
        public IReadOnlyReactiveProperty<int> Score => _score;
        public IReadOnlyReactiveProperty<int> CurrentCombo => _currentCombo;
        public IReadOnlyReactiveProperty<int> MaxCombo => _maxCombo;

        public readonly Subject<JudgementViewModel> JudgeNotification  = new();
        
        public GameModel(GamePresenter presenter)
        {
            _presenter = presenter;
            _beatmap = null;
        }

        public void Initialize()
        {
            _judgeList = new Dictionary<JudgementType, int>();
            _score.Value = 0;
            _currentCombo.Value = 0;
            _maxCombo.Value = 0;
        }

        private bool IsInsideJudgementArea(JudgementType judgementType, float distance)
        {
            if (judgementType == JudgementType.Miss)
            {
                return true;
            }
            return distance < GameConst.JudgementByDistance[judgementType];
        }
        
        public void DoJudge(int laneId)
        {
            var nearestNotePresenter = _presenter.NotePresenters.GetNearestNotePresenter(laneId);
            if (nearestNotePresenter == null)
            {
                return;
            }

            var distance = Math.Abs(nearestNotePresenter.ZPosition);
            if (IsInsideJudgementArea(JudgementType.Perfect, distance))
            {
                AddJudgement(JudgementType.Perfect);
            } 
            else if (IsInsideJudgementArea(JudgementType.Good, distance))
            {
                AddJudgement(JudgementType.Good);
            } 
            else if (IsInsideJudgementArea(JudgementType.Bad, distance))
            {
                AddJudgement(JudgementType.Bad);
            }
            else
            {
                AddJudgement(JudgementType.Miss);
            }
            
            if (IsInsideJudgementArea(JudgementType.Bad, distance)) {
                _presenter.SpawnParticle(laneId);
            }
        }
        
        private void AddJudgement(JudgementType type)
        {
            //判定リストに判定を追加する
            if (!_judgeList.ContainsKey(type))
            {
                _judgeList.Add(type, 0);
            }

            _judgeList[type]++;

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
            var point = GetPointForJudge(type);
            var bonus = CalcBonusAmount(point);
            _score.Value += point + bonus;
            
            _presenter.UpdateComboCount(_currentCombo.Value);
        }

        private int CalcBonusAmount(int value)
        {
            return value * _currentCombo.Value;
        }
        
        private static int GetPointForJudge(JudgementType type)
        {
            if (!GameConst.PointForJudge.ContainsKey(type))
            {
                throw new KeyNotFoundException();
            }

            return GameConst.PointForJudge[type];
        }
    }
}