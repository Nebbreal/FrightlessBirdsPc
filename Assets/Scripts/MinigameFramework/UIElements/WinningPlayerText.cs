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

        public void UpdateWinningPlayer(Component sender, object[] winningPlayerObj)
        {
            if (winningPlayerObj.First() is string winningPlayer)
            {
                _winningPlayerText.text = winningPlayer + " wins!";
            }
        }
    }
}
