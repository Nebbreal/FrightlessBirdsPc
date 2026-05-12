using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace MinigameFramework.UIElements
{
    public class ScoreboardPlayerScoreText : MonoBehaviour
    {
        private TMP_Text _scoreboardPlayerScoreText;
        public int id;

        public void Awake()
        {
            _scoreboardPlayerScoreText = GetComponent<TMP_Text>();
            ScoreboardManager scm = GameManager.Instance.GetComponentInChildren<ScoreboardManager>();
            if (GameManager.Instance.PlayerManager.ActivePlayers.All(o => o.GetComponent<PlayerController>().ID != id))
            {
                _scoreboardPlayerScoreText.text = " ";
            }
            else
            {
                String pname = GameManager.Instance.PlayerManager.Players[id].name;
                _scoreboardPlayerScoreText.text = scm.PlayerScores[pname].ToString();
            }
        }

        public void UpdateWinningPlayerScore(Component sender, object[] _obj)
        {
            if (_scoreboardPlayerScoreText.text == " ")
            {
                return;
            }
            ScoreboardManager scm = GameManager.Instance.GetComponentInChildren<ScoreboardManager>();
            String pname = GameManager.Instance.PlayerManager.PlayerNames[id];
            _scoreboardPlayerScoreText.text = scm.PlayerScores[pname].ToString();
        }
    }
}
