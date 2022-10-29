using System.Collections.Generic;
using App.Application;
using App.Domain.Ingame;
using App.Presentation.Ingame.Presenters;
using UnityEngine;

namespace App.Domain
{
    public class GameManager
    {
        private static GameManager _instance = new GameManager();

        public static bool ShouldPlayCutIn = true;

        public static GameManager GetInstance()
        {
            return _instance;
        }

        public GameModel CurrentGame { get; private set; }

        private readonly List<GameResultViewModel> _resultList = new();

        private GameManager()
        {
            CurrentGame = null;
        }

        public GameModel StartGame(GamePresenter presenter)
        {
            CurrentGame = new GameModel(presenter);
            CurrentGame.Initialize();
            return CurrentGame;
        }

        public void AddResultViewModel(GameResultViewModel resultViewModel)
        {
            _resultList.Add(resultViewModel);
        }

        public IReadOnlyList<GameResultViewModel> GetResultList()
        {
            return _resultList;
        }
    }
}