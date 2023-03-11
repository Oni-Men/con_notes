using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database.Impl
{
    [CreateAssetMenu(fileName = "FlagDatabase.asset", menuName = "FlagDatabaseを作成する")]
    public class ScriptableFlagDatabase : ScriptableObject, IDatabase<bool>
    {
        [Serializable]
        public class FlagData
        {
            public string flagId;
            public bool value;
        }

        [SerializeField]
        private List<FlagData> flags = new();

        private Dictionary<string, bool> _flagDictionary;
        private int _flagHash = -1;
        
        private Dictionary<string, bool> GetDictionary()
        {
            var currentFlagHash = flags.GetHashCode();
            if (currentFlagHash != _flagHash)
            {
                UpdateDictionary();
                _flagHash = currentFlagHash;
            }
            return _flagDictionary;
        }

        private void UpdateDictionary()
        {
            _flagDictionary = new();
            foreach (var flagData in flags)
            {
                _flagDictionary[flagData.flagId] = flagData.value;
            }
        }

        public IReadOnlyDictionary<string, bool> All()
        {
            return GetDictionary();
        }

        public void Put(string key, bool value)
        {
            GetDictionary()[key] = value;
        }

        public void Delete(string key)
        {
            GetDictionary().Remove(key);
        }

        public bool Has(string key)
        {
            return GetDictionary().ContainsKey(key) && GetDictionary()[key];
        }
    }
}