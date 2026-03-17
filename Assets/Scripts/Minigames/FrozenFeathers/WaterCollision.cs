using UnityEngine;
using UnityEngine.Serialization;

namespace Minigames.FrozenFeathers
{
    public class WaterCollision : MonoBehaviour
    {
        
        [SerializeField]
        private AudioSource splashSource;
        [SerializeField]
        private LayerMask lMask;
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != lMask)
            {
                splashSource.Play();
            }
        }
    }
}
