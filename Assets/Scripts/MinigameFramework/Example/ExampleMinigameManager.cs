using System.Collections.Generic;
using EventSystem;
using Minigames;
using UnityEngine;

namespace MinigameFramework.Example
{
    public class ExampleMinigameManager : MonoBehaviour
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
                PlayerController playerController = player.GetComponent<PlayerController>();
                
                playerBody.linearVelocity = Vector3.zero;
                playerTransform.position = initialSpawnPoints[playerController.ID].transform.position;
                playerTransform.rotation = Quaternion.LookRotation(-initialSpawnPoints[playerController.ID].transform.position);
            }
            
            onSetAllPlayerLives.Raise(this, _gameConfig.maxLives);
        }
    }
}
