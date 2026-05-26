using System;
using System.Linq;
using UnityEngine;
using EventSystem;

namespace MinigameFramework.UIElements
{
    public class ScoreboardPlayerScoreBar : MonoBehaviour
    {
        private RectTransform _rectTransform;
        public int id;
        public float animationDuration;
        public float endDelay;
        private float _animationTime = 1f;
        public float base_width = 520f;
        private float start_width = 520f;
        private float target_width = 520f;
        public float hidden_width = 100f;
        public float scale_diff_per_win = 200f;
        public GameEvent onCleanScoreboard;
        private bool isActive;

        public void OnEnable()
        {
            _rectTransform = GetComponent<RectTransform>();
            var activePlayers = GameManager.Instance.PlayerManager.ActivePlayers;
            if (activePlayers.All(o => o.GetComponent<PlayerController>().ID != id))
            {
                var rect = _rectTransform.rect;
                _rectTransform.sizeDelta = new Vector2(hidden_width, rect.height);
            } else
            {
                var rect = _rectTransform.rect;
                _rectTransform.sizeDelta = new Vector2(base_width, rect.height);
            }

            isActive = false;
        }

        public void Update()
        {
            _animationTime += Time.deltaTime;

            float t = Ease.InOutCubic(MathF.Min(_animationTime / animationDuration, 1f));

            var rect = _rectTransform.rect;
            var width = start_width + (target_width - start_width) * t;
            _rectTransform.sizeDelta = new Vector2(width, rect.height);

            if (_animationTime >= animationDuration + endDelay && isActive)
            {
                isActive = false;
                onCleanScoreboard.Raise(this);
            }
        }

        public void UpdateWinningPlayerScore(Component sender, object[] _obj)
        {
            ScoreboardManager scoreboardMan = GameManager.Instance.GetComponentInChildren<ScoreboardManager>();

            start_width = _rectTransform.rect.width;
            target_width = base_width + scoreboardMan.PlayerScores[id] * scale_diff_per_win;
            if (Math.Abs(start_width - target_width) < Single.Epsilon)
                return;
            isActive = true;
            _animationTime = 0f;
        }
    }
}
