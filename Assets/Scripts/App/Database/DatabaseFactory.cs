using System.Collections.Generic;
using Database.Impl;
using UnityEngine;
using Object = UnityEngine.Object;

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