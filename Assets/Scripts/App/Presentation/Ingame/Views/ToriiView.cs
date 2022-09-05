using UnityEngine;

namespace App.Presentation.Ingame.Views
{
    public class ToriiView : MonoBehaviour
    {
        
        [SerializeField] private float speed;

        [SerializeField] private float lifetime;
        
        public float Speed => speed;

        public float Lifetime => lifetime;
        
        void Update()
        {
            transform.Translate(0, 0, Time.deltaTime * -speed);
            if (transform.position.z < -5)
            {
                Destroy(gameObject);
            }
        }
    }
}