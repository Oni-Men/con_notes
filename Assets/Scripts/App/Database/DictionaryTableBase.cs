using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Database
{
    public abstract class DictionaryDatabaseBase<T> : ScriptableObject, IDatabase<T>
    {
        [Serializable]
        public class Column
        {
            public string key;
            public T value;
        }

        [SerializeField]
        private List<Column> columns = new();

        private int _columnsHash = -1;
        private IReadOnlyDictionary<string, T> _table;

        protected IReadOnlyDictionary<string, T> ToDict()
        {
            UpdateDictionary();
            return _table;
        }

        private void UpdateDictionary()
        {
            var currentFlagHash = -1;
            if (_table != null)
            {
                currentFlagHash = _table.GetHashCode();
                if (currentFlagHash == _columnsHash) return;
            }

            _columnsHash = currentFlagHash;
            var table = new Dictionary<string, T>();
            foreach (var column in columns)
            {
                table[column.key] = column.value;
            }

            _table = table;
        }

        public IReadOnlyDictionary<string, T> All()
        {
            return ToDict();
        }

        public T Get(string key)
        {
            UpdateDictionary();
            return _table[key];
        }

        public void Put(string key, T value)
        {
            columns.Add(new Column
            {
                key = key,
                value = value
            });
        }

        public void Delete(string key)
        {
            var match = columns.Find(c => c.key == key);
            columns.Remove(match);
        }

        public bool Has(string key)
        {
            UpdateDictionary();
            return _table.ContainsKey(key);
        }
    }
}