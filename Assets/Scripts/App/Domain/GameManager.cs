using System.Collections.Generic;
using App.Application;
using App.Domain.Ingame;
using App.Presentation.Ingame.Presenters;

namespace App.Domain
{
    public class GameManager
    {
        public static GameManager Instance { get; } = new GameManager();

        public static bool ShouldPlayCutIn = false;

        public static GameManager GetInstance()
        {
            return Instance;
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