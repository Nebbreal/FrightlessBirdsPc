using TMPro;
using UnityEngine;

namespace EventSystem.Examples
{
    public class EventListenerExample : MonoBehaviour
    {
        private int _currentCount = 0;
        [SerializeField]
        private TMP_Text text;
        
        public void OnGameEventExample(Component sender, params object[] data)
        {
            _currentCount++;
            text.text = _currentCount.ToString();
        }
    }
}
