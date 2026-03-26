using System.Linq;
using TMPro;
using UnityEngine;

namespace MinigameFramework.UIElements
{
    public class CountdownTimer : MonoBehaviour
    {
        private TMP_Text _timerText;

        public void Start()
        {
            _timerText = GetComponent<TMP_Text>();
        }

        public void UpdateTimer(Component sender, object[] timeRemaining)
        {
            if (timeRemaining.First() is string timeRemainingStr)
            {
                _timerText.text = timeRemainingStr;
            }
        }
    }
}
