using System.Collections.Generic;
using App.Application;

namespace App.Domain
{
    public class GameManager
    {
        public static GameManager Instance { get; } = new GameManager();

        public static GameManager GetInstance()
        {
            return Instance;
        }

        private readonly List<GameResultViewModel> _resultList = new();

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