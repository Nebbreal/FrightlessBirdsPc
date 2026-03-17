using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Minigames.FrozenFeathers
{
    public class MockPlayerManager : MonoBehaviour
    {
        public List<GameObject> players;
        
        [NonSerialized]
        public Dictionary<string, int> PlayerLives = new();
        [NonSerialized]
        public List<GameObject> AlivePlayers;
        private GameConfig _gameConfig;

        public void Start()
        {
            _gameConfig = gameObject.GetComponent<GameConfig>();
        }

        public void SetPlayerLives()
        {
            AlivePlayers = players;
            foreach (var player in players)
            {
                if (PlayerLives.ContainsKey(player.name))
                {
                    PlayerLives[player.name] = _gameConfig.playerLives;
                }
                PlayerLives.Add(player.name, _gameConfig.playerLives);
            }
        }

        public void DeductLife(GameObject player)
        {
            PlayerLives[player.name]--;
            if (PlayerLives[player.name] == 0)
            {
                AlivePlayers.Remove(player);
                player.SetActive(false);
            }
        }

        public int GetAlivePlayerAmount()
        {
            int alivePlayers = AlivePlayers.Count;
            return alivePlayers;
        }
    }
}
