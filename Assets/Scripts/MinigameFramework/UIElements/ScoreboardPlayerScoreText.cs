using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace MinigameFramework.UIElements
{
    public class ScoreboardPlayerScoreText : MonoBehaviour
    {
        private TMP_Text _scoreboardPlayerScoreText;

        public void Awake()
        {
            _scoreboardPlayerScoreText = GetComponent<TMP_Text>();
        }

        public void UpdateWinningPlayerScore(Component sender, object[] winnerDataObj)
        {
            if (winnerDataObj.First() is GameObject[] winningPlayers)
            {
                ScoreboardManager scm = GameManager.Instance.GetComponentInChildren<ScoreboardManager>();
                foreach (GameObject p in winningPlayers)
                {
                    _scoreboardPlayerScoreText.text = scm.PlayerScores[p.name].ToString();
                }
            }
        }
    }
}
