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
                _players = _playerManager.Players;
            #endregion
            
            foreach (GameObject player in _players)
            {
                Rigidbody playerBody = player.GetComponent<Rigidbody>();
                Transform playerTransform = player.GetComponent<Transform>();
                switch (player.name)
                {
                    case "Player1":
                        playerBody.linearVelocity = Vector3.zero;
                        playerTransform.position = initialSpawnPoints[0].transform.position;
                        playerTransform.rotation = Quaternion.LookRotation(-initialSpawnPoints[0].transform.position);
                        break;
                    case "Player2":
                        playerBody.linearVelocity = Vector3.zero;
                        playerTransform.position = initialSpawnPoints[1].transform.position;
                        playerTransform.rotation = Quaternion.LookRotation(-initialSpawnPoints[1].transform.position);
                        break;
                    case "Player3":
                        playerBody.linearVelocity = Vector3.zero;
                        playerTransform.position = initialSpawnPoints[2].transform.position;
                        playerTransform.rotation = Quaternion.LookRotation(-initialSpawnPoints[2].transform.position);
                        break;
                    case "Player4":
                        playerBody.linearVelocity = Vector3.zero;
                        playerTransform.position = initialSpawnPoints[3].transform.position;
                        playerTransform.rotation = Quaternion.LookRotation(-initialSpawnPoints[3].transform.position);
                        break;
                }
            }
            
            onSetAllPlayerLives.Raise(this, _gameConfig.maxLives);
        }
    }
}
