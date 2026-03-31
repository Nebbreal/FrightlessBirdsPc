using UnityEngine;

namespace EventSystem.Examples
{
    public class EventTriggerExample : MonoBehaviour
    {
        [SerializeField]
        private GameEvent gameEvent;

        public void OnTriggerEnter(Collider other)
        {
            Debug.Log("Trigger entered by " + other.name);
            gameEvent.Raise(this, null);
        }
    }
}