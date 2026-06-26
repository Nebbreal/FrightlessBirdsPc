using System;
using System.Collections.Generic;
using System.Linq;
using MinigameFramework;
using UnityEngine;

namespace Minigames.FrozenFeathers
{
    public class MockPlayerManager : MonoBehaviour
    {
        [NonSerialized]
        public List<GameObject> Players;
        
        [NonSerialized]
        public Dictionary<string, int> PlayerLives = new();
        [NonSerialized]
        public List<GameObject> AlivePlayers;
        private GameConfig _gameConfig;

        public void Start()
        {
            Players = GameObject.FindGameObjectsWithTag("Player").ToList();
            _gameConfig = gameObject.GetComponent<GameConfig>();
        }

        public void SetPlayerLives()
        {
            AlivePlayers = Players;
            foreach (var player in Players)
            {
                player.SetActive(true);
                PlayerLives[player.name] = GameManager.Instance.CurrentMinigame.maxLives;
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
