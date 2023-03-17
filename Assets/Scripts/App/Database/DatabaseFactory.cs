using Database.Impl;
using UnityEngine;

namespace Database
{
    public class DatabaseFactory
    {
        private static SheetScenarioDatabase _scenarioDatabase = null;

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
    }
}