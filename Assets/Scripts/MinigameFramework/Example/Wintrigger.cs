using EventSystem;
using UnityEngine;

namespace MinigameFramework.Example
{
    public class Wintrigger : MonoBehaviour
    {
        public GameEvent onEndMinigame;
        private bool _hasEnded;

        public void Start()
        {
            _hasEnded = false;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (_hasEnded) return;
       
            onEndMinigame.Raise(this, other.gameObject);
            _hasEnded = true;
        }
    }
}
