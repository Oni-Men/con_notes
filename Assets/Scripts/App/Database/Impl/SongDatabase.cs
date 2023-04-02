using Database;
using UnityEngine;

namespace App.Database.Impl
{
    [CreateAssetMenu(fileName = "SongDatabase.asset", menuName = "SongDatabaseを作成する")]
    public class SongDatabase : DictionaryDatabaseBase<string>
    {
    }
}