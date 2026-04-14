using UnityEngine;
using EventSystem;
using Minigames;

public class ScoreTrigger : MonoBehaviour
{
    public class Wintrigger : MonoBehaviour
    {
        public GameEvent onScorePoint;
        public int side = 0;
        private bool _hasEnded;

        public void Start()
        {
            _hasEnded = false;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (_hasEnded) return;
       
            onScorePoint.Raise(this, other.gameObject);
            _hasEnded = true;
        }
    }
}
