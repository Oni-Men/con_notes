using App.Domain;
using App.Presentation.Ingame.Views;
using UnityEngine;

namespace App.Common
{
    public class Generator : MonoBehaviour
    {
        [SerializeField] private NoteView prefab;
    
        public void Awake()
        {
        }

        public void Update()
        {
            if (Input.GetKeyDown("1"))
            {
                SpawnNote(-2);
            }
            if (Input.GetKeyDown("2"))
            {
                SpawnNote(-1);
            }
            if (Input.GetKeyDown("3"))
            {
                SpawnNote(0);
            }
            if (Input.GetKeyDown("4"))
            {
                SpawnNote(1);
            }
            if (Input.GetKeyDown("5"))
            {
                SpawnNote(2);
            }
        
        }

        void SpawnNote(int laneId)
        {
            var x = laneId * GameConst.NoteWidth;
            var z = prefab.Speed * GameConst.Lifetime;
            Instantiate(prefab, new Vector3(x, 0 ,z), Quaternion.identity);
        }

    }
}

