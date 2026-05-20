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
        private ScoreboardManager _scoreboardMan;
        public int id;
        public float animationDuration;
        private float _animationTime = 1f;

        public void Awake()
        {
            _scoreboardPlayerScoreText = GetComponent<TMP_Text>();
            _scoreboardMan = GameManager.Instance.GetComponentInChildren<ScoreboardManager>();
            if (GameManager.Instance.PlayerManager.ActivePlayers.All(o => o.GetComponent<PlayerController>().ID != id))
            {
                _scoreboardPlayerScoreText.text = " ";
            }
        }

        public void Update()
        {
            if (_animationTime <= 0.5f && _animationTime + Time.deltaTime > 0.5)
            {
                _scoreboardPlayerScoreText.text = _scoreboardMan.PlayerScores[id].ToString();
            }
            _animationTime += Time.deltaTime;
            float t = _animationTime / animationDuration;
            float eased_t = Ease.InLinearOutCubic(t);

            transform.localScale = new Vector3(eased_t, eased_t, 1f);
        }

        public void UpdateWinningPlayerScore(Component sender, object[] _obj)
        {
            if (_scoreboardPlayerScoreText.text == " " || _scoreboardPlayerScoreText.text == _scoreboardMan.PlayerScores[id].ToString())
            {
                return;
            }

            _animationTime = 0f;
        }
    }
}
