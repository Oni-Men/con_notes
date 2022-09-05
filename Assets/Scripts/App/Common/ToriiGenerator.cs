using App.Domain;
using App.Presentation.Ingame.Views;
using UnityEngine;

namespace App.Common
{
    public class ToriiGenerator : MonoBehaviour
    {

        [SerializeField] private ToriiView toriiPrefab;
        
        void SpawnTroii()
        {
            var z = toriiPrefab.Speed * GameConst.Lifetime;
            Instantiate(toriiPrefab, new Vector3(0, 0, z), Quaternion.identity);
        }
    }
}