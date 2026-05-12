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
        public float animationDuration;
        private float _animationTime = 1f;

        public void Awake()
        {
            _scoreboardPlayerScoreText = GetComponent<TMP_Text>();
            ScoreboardManager scm = GameManager.Instance.GetComponentInChildren<ScoreboardManager>();
            if (GameManager.Instance.PlayerManager.ActivePlayers.All(o => o.GetComponent<PlayerController>().ID != id))
            {
                _scoreboardPlayerScoreText.text = " ";
            }
        }

        public void Update()
        {
            if (_animationTime <= 0.5f && _animationTime + Time.deltaTime > 0.5)
            {
                ScoreboardManager scm = GameManager.Instance.GetComponentInChildren<ScoreboardManager>();
                _scoreboardPlayerScoreText.text = scm.PlayerScores[id].ToString();
            }
            _animationTime += Time.deltaTime;
            float t = _animationTime / animationDuration;

            // _scoreboardPlayerScoreText.fontSize = ease(t) * 128f;
            transform.localScale = new Vector3(ease(t), ease(t), 1f);
            // transform.lossyScale.Set(ease(t), ease(t), ease(t));
        }

        public void UpdateWinningPlayerScore(Component sender, object[] _obj)
        {
            ScoreboardManager scm = GameManager.Instance.GetComponentInChildren<ScoreboardManager>();
            if (_scoreboardPlayerScoreText.text == " " || _scoreboardPlayerScoreText.text == scm.PlayerScores[id].ToString())
            {
                return;
            }

            _animationTime = 0f;
        }

        private float ease(float x)
        {
            x = (MathF.Min(x, 1f) - 0.5f) * 2f;
            if (x < 0)
            {
                return -x;
            }

            return easeOutCubic(x);
        }

        private float easeOutCubic(float x)
        {
            return 1f - MathF.Pow(1f - x, 3f);
        }
    }
}
