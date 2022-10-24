using App.Domain.Ingame;
using App.Presentation.Ingame.Presenters;

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
        
        
    }
}