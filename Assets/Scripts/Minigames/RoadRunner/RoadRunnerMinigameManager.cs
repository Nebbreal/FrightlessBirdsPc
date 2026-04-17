using UnityEngine;
using EventSystem;
using Minigames;
using MinigameFramework;
using System.Collections.Generic;
using System.Linq;

namespace Minigames.RoadRunner
{
    public class RoadRunnerMinigameManager : MonoBehaviour
    {
        //General Config
        private GameObject _gameManager;
        private Minigame _gameConfig;
        private PlayerManager _playerManager;
        private List<GameObject> _players;

        public GameEvent onEndMinigame;
        [SerializeField] 
        private GameObject deathPoint;

        
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

        public void EliminatePlayer(Component sender, object[] entities)
        {
            if (entities.First() is GameObject entity)
            {
                if (!_players.Contains(entity)) return;

                _playerManager.DeductLife(entity);

                if (_playerManager.PlayerLives[entity.name] > 0)
                {
                    GameObject playerObject = entity;
                    PlayerController player = entity.GetComponent<PlayerController>();
                    Rigidbody playerRigidbody = playerObject.GetComponent<Rigidbody>();
                    playerRigidbody.useGravity = true;
                    playerRigidbody.linearDamping = 1f;
                    playerRigidbody.linearVelocity = Vector3.zero;

                    playerObject.transform.position = initialSpawnPoints[player.id].transform.position;
                }
                else
                {
                    Rigidbody rb = entity.GetComponent<Rigidbody>();
                    rb.linearVelocity = Vector3.zero;
                    entity.transform.position = deathPoint.transform.position;

                    rb.useGravity = true;
                    rb.linearDamping = 1f;
                }


                if (_playerManager.GetAlivePlayerAmount() <= 1)
                {
                    OnWin();
                }
            }
        }

        private void OnWin()
        {
            onEndMinigame.Raise(this, _playerManager.AlivePlayers.First());
        }
    }
}
