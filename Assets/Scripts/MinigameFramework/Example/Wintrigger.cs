using System.Linq;
using EventSystem;
using UnityEngine;

namespace MinigameFramework.Example
{
    public class Wintrigger : MonoBehaviour
    {
        public GameEvent onEndMinigame;
        private bool _hasEnded;
        private PlayerManager _playerManager;

        public void Start()
        {
            _hasEnded = false;
            _playerManager = GameManager.Instance.PlayerManager;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (_hasEnded) return;

            if (_playerManager.Players.Contains(other.gameObject))
            {
                Team winningTeam = _playerManager.Teams.First(t => t.Players.Contains(other.gameObject));

                GameObject[] winningTeamPlayers = winningTeam.Players.ToArray();
                object[] data = winningTeamPlayers.Cast<object>().ToArray();
                onEndMinigame.Raise(this, data);
            }
            else
            {
                onEndMinigame.Raise(this, other.gameObject);
            }
       
            _hasEnded = true;
        }
    }
}
