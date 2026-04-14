using UnityEngine;
using EventSystem;
using Minigames;
using MinigameFramework;
using System.Collections.Generic;

namespace Minigames.RoadRunner
{
    public class RoadRunnerMinigameManager : MonoBehaviour
    {
        private Minigame _gameConfig;
        private PlayerManager _playerManager;
        private List<GameObject> _players;
        
        [SerializeField] 
        private List<GameObject> initialSpawnPoints;
        [SerializeField] 
        private GameEvent onSetAllPlayerLives;
        public void Start()
        {
            #region MinigameConfig
                _gameConfig = GameManager.Instance.CurrentMinigame;
                
                _playerManager = GameManager.Instance.PlayerManager;
                _players = _playerManager.ActivePlayers;
            #endregion
            
            foreach (GameObject player in _players)
            {
                Rigidbody playerBody = player.GetComponent<Rigidbody>();
                Transform playerTransform = player.GetComponent<Transform>();
                PlayerController p = player.GetComponent<PlayerController>();
                playerBody.linearVelocity = Vector3.zero;
                playerTransform.position = initialSpawnPoints[p.id].transform.position;
                playerTransform.rotation = Quaternion.LookRotation(-initialSpawnPoints[p.id].transform.position);

                // player.AddComponent();
            }
            
            onSetAllPlayerLives.Raise(this, _gameConfig.maxLives);
        }
    }
}
