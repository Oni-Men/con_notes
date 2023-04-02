using App.Database.Impl;
using Database.Impl;
using UnityEngine;

namespace App.Database
{
    public static class DatabaseFactory
    {
        private static SheetScenarioDatabase _scenarioDatabase = null;
        private static SongDatabase _songDatabase = null;
        
        public static SheetScenarioDatabase ScenarioDatabase
        {
            get
            {
                if (_scenarioDatabase == null)
                {
                    _scenarioDatabase = Resources.Load<SheetScenarioDatabase>("Database/SheetScenarioDatabase");
                }

                return _scenarioDatabase;
            }
        }

        public static SongDatabase SongDatabase
        {
            get
            {
                if (_songDatabase == null)
                {
                    _songDatabase = Resources.Load<SongDatabase>("Database/SongDatabase");
                }

                return _songDatabase;
            }
        }
    }
}