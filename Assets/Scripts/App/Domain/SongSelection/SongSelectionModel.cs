using System.Collections.Generic;
using App.Domain;

namespace App.Domain.SongSelection
{
    public class SongSelectionModel
    {
        private static SongSelectionModel _instance;
     
        private List<Beatmap> _beatmaps;

        public IReadOnlyList<Beatmap> Beatmaps => _beatmaps;

        public static SongSelectionModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SongSelectionModel();
            }
            return _instance;
        }
        
        private SongSelectionModel()
        {
            _beatmaps = new List<Beatmap>();
        }

        public void LoadBeatmaps()
        {
            //TODO
        }

        public void SelectSong(Beatmap map)
        {
            //TODO
        }
        
    }
}