using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace MinigameFramework.UIElements
{
    public class WinningPlayerText : MonoBehaviour
    {
        private TMP_Text _winningPlayerText;

        public void Awake()
        {
            _winningPlayerText = GetComponent<TMP_Text>();
        }

        public void UpdateWinningPlayer(Component sender, object[] winnerDataObj)
        {
            if (winnerDataObj[0] is string winningPlayers && winnerDataObj[1] is Color winnerColor)
            {
                _winningPlayerText.color = winnerColor;
                _winningPlayerText.text = winningPlayers;
            }
        }
    }
}
