using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minigames.FrozenFeathers
{
    public class MockPlayerManager : MonoBehaviour
    {
        public List<GameObject> players;
        
        [NonSerialized]
        public Dictionary<string, int> PlayerLives = new Dictionary<string, int>();
        private GameConfig _gameConfig;

        public void Start()
        {
            _gameConfig = gameObject.GetComponent<GameConfig>();
        }

        public void SetPlayerLives()
        {
            foreach (var player in players)
            {
                if (PlayerLives.ContainsKey(player.name))
                {
                    PlayerLives[player.name] = _gameConfig.playerLives;
                }
                PlayerLives.Add(player.name, _gameConfig.playerLives);
            }
        }
    }
}
