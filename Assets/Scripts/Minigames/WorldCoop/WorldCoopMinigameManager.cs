using UnityEngine;
using EventSystem;
using Minigames;
using MinigameFramework;
using System.Collections.Generic;
using System.Linq;

namespace Minigames.WorldCoop
{
    public class WorldCoopMinigameManager : MonoBehaviour
    {
        //General Config
        private GameObject _gameManager;
        private Minigame _gameConfig;
        private PlayerManager _playerManager;
        private List<GameObject> _players;

        private int scoreTeam1 = 0;
        private int scoreTeam2 = 0;

        public GameEvent onEndMinigame;
        [SerializeField] 
        private GameObject deathPoint;
        
        [SerializeField] 
        private List<GameObject> initialSpawnPoints;
        [SerializeField] 
        private GameEvent onSetAllPlayerLives;

        [SerializeField] 
        private int scoreToWin = 3;
        [SerializeField] 
        private GameObject ballObject;
        [SerializeField] 
        private Vector3 ballInitialSpawn;

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

        public void OnGoalScored(Component sender, object[] data)
        {
            if (data != null && data.Length > 0 && data[0] is int)
            {
                int scoringTeam = (int)data[0];

                if (scoringTeam == 1) {
                    scoreTeam1++;
                } else if (scoringTeam == 2) {
                    scoreTeam2++;
                }
                
                Debug.Log($"Score Update! Team 1: {scoreTeam1} | Team 2: {scoreTeam2}");
            }

            ResetRound();
        }

        private void ResetRound()
        {
            if (ballObject != null)
            {
                Rigidbody ballRb = ballObject.GetComponent<Rigidbody>();
                ballRb.linearVelocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
                ballObject.transform.position = ballInitialSpawn;
            }

            foreach (GameObject player in _players)
            {
                Rigidbody playerBody = player.GetComponent<Rigidbody>();
                Transform playerTransform = player.GetComponent<Transform>();
                PlayerController p = player.GetComponent<PlayerController>();
                playerBody.linearVelocity = Vector3.zero;
                playerTransform.position = initialSpawnPoints[p.id].transform.position;
                playerTransform.rotation = Quaternion.LookRotation(-initialSpawnPoints[p.id].transform.position);
            }

            if (scoreTeam1 >= scoreToWin || scoreTeam2 >= scoreToWin)
            {
                OnWin();
            }
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
                    playerRigidbody.linearVelocity = Vector3.zero;

                    playerObject.transform.position = initialSpawnPoints[player.id].transform.position;
                }
                else
                {
                    entity.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                    entity.transform.position = deathPoint.transform.position;
                }

                if (scoreTeam1 >= scoreToWin || scoreTeam2 >= scoreToWin)
                {
                    OnWin();
                }
            }
        }

        private void OnWin()
        {
            if (scoreTeam1 >= scoreToWin)
            {
                GameObject[] team1Players = _playerManager.Teams[0].Players.ToArray();
                onEndMinigame.Raise(this, team1Players);
            }
            else if (scoreTeam2 >= scoreToWin)
            {
                GameObject[] team2Players = _playerManager.Teams[1].Players.ToArray();
                onEndMinigame.Raise(this, team2Players);
            }
        }
    }
}