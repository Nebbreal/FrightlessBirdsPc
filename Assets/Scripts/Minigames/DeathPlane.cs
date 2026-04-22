using EventSystem;

using UnityEngine;

namespace Minigames
{
    public class DeathPlane : MonoBehaviour
    {
        [SerializeField]
        private GameEvent OnEliminatePlayer;

        public void OnTriggerEnter(Collider other)
        {
            OnEliminatePlayer.Raise(this, other.gameObject);
        }
    }
}