using System.Collections.Generic;
using System.Linq;
using App.Domain;
using App.Presentation.Ingame.Views;
using UnityEngine;

namespace App.Common
{
    public class Generator : MonoBehaviour
    {
        [SerializeField] private NoteView prefab;

        public void SpawnNote(int laneId)
        {
            var x = laneId * GameConst.NoteWidth;
            var z = prefab.Speed * GameConst.Lifetime;
            var position = new Vector3(x, 0, z);
            Instantiate(prefab, position, Quaternion.identity);
        }
    }
}