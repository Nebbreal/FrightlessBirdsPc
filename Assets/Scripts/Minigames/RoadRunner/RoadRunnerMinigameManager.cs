using System;
using UnityEngine;
using EventSystem;
using Minigames;
using MinigameFramework;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace Minigames.RoadRunner
{
    public class RoadRunnerMinigameManager : MonoBehaviour
    {
        //General Config
        private GameObject _gameManager;
        private Minigame _gameConfig;
        private PlayerManager _playerManager;
        private List<GameObject> _players;
        public List<TextMeshProUGUI> _playerScoreboards;

        [NonSerialized] 
        private Dictionary<string, int> playerScores = new();
        [NonSerialized] 
        public Dictionary<string, int> playerSides = new();

        public GameEvent onEndMinigame;
        [SerializeField] 
        private GameObject deathPoint;

        private float timer = 12f;

        
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

            playerSides = new();
            foreach (GameObject player in _players)
            {
                PlayerController p = player.GetComponent<PlayerController>();
                playerSides.Add(player.name, p.id is > 0 and < 3 ? 1 : 0);
                playerScores.Add(player.name, 0);
            }

            timer = _gameConfig.roundLength;
            
            onSetAllPlayerLives.Raise(this, _gameConfig.maxLives);
        }

        public void Update()
        {
            timer -= Time.deltaTime;

            if (timer < 0f && !GameManager.Instance._hasMinigameEnded)
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
                    playerSides[player.name] = player.id is > 0 and < 3 ? 1 : 0;
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
            List<GameObject> winners = new();
            var highestScore = 0;
            foreach (GameObject player in _players)
            {
                var score = playerScores[player.name];
                if (score > highestScore)
                {
                    winners.Clear();
                    winners.Add(player);
                    highestScore = score;
                } else if (score == highestScore)
                {
                    winners.Add(player);
                }
            }

            onEndMinigame.Raise(this, winners.ToArray());
        }

        public void OnScorePoint(Component sender, object[] entities)
        {
            if (entities.First() is GameObject entity)
            {
                if (!_players.Contains(entity)) return;

                PlayerController player = entity.GetComponent<PlayerController>();
                int newScore = playerScores[player.name] + 1;
                playerScores[player.name] = newScore;

                _playerScoreboards[player.id].text = newScore.ToString();
            }
        }
    }
}
