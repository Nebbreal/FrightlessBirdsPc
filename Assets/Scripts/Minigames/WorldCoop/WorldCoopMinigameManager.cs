using UnityEngine;
using EventSystem;
using Minigames;
using MinigameFramework;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

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
        private bool overtimeActive = false;

        private float timer = 12f;

        public GameEvent onEndMinigame;
        
        [SerializeField] private List<GameObject> initialSpawnPoints;
        [SerializeField] private GameEvent onSetAllPlayerLives;

        [SerializeField] private int scoreToWin = 3;
        [SerializeField] private GameObject ballObject;
        [SerializeField] private Vector3 initialBallSpawn;

        [Header("Scoreboard Visuals")]
        [SerializeField] private List<MeshRenderer> team1ScoreBalls; 
        [SerializeField] private List<MeshRenderer> team2ScoreBalls; 
        [SerializeField] private float fadeSpeed = 2f;
        [SerializeField] private float scoreStartOpacity = 0.2f;

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
                playerTransform.position = initialSpawnPoints[p.ID].transform.position;
                playerTransform.rotation = Quaternion.LookRotation(-initialSpawnPoints[p.ID].transform.position);

                // player.AddComponent();
            }
            
            timer = _gameConfig.roundLength;

            onSetAllPlayerLives.Raise(this, _gameConfig.maxLives);

            InitializeScoreboardBalls(team1ScoreBalls);
            InitializeScoreboardBalls(team2ScoreBalls);
        }

        private void InitializeScoreboardBalls(List<MeshRenderer> balls)
        {
            foreach (var ballRenderer in balls)
            {
                foreach (var mat in ballRenderer.materials)
                {
                    Color c = mat.color;
                    c.a = scoreStartOpacity; 
                    mat.color = c;
                }
            }
        }

        public void OnGoalScored(Component sender, object[] data)
        {
            if (data == null || data.Length == 0 || data[0] is not int scoringTeam) return;

            if (overtimeActive)
            {
                OnWin(scoringTeam);
                return;
            }

            if (scoringTeam == 1 && scoreTeam1 < team1ScoreBalls.Count)
            {
                StartCoroutine(FadeInBall(team1ScoreBalls[scoreTeam1]));
                scoreTeam1++;
            } 
            else if (scoringTeam == 2 && scoreTeam2 < team2ScoreBalls.Count)
            {
                StartCoroutine(FadeInBall(team2ScoreBalls[scoreTeam2]));
                scoreTeam2++;
            }
            ResetRound();
        }

        private IEnumerator FadeInBall(MeshRenderer ballRenderer)
        {
            Material[] mats = ballRenderer.materials;
            float alpha = mats[0].color.a;

            while (alpha < 1f)
            {
                alpha += Time.deltaTime * fadeSpeed;
                foreach (var mat in mats)
                {
                    Color c = mat.color;
                    c.a = Mathf.Clamp01(alpha);
                    mat.color = c;
                }
                yield return null;
            }
        }

        private void ResetRound()
        {
            if (ballObject != null)
            {
                Rigidbody ballRb = ballObject.GetComponent<Rigidbody>();
                ballRb.linearVelocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
                ballObject.transform.position = initialBallSpawn;
            }

            foreach (GameObject player in _players)
            {
                Rigidbody playerBody = player.GetComponent<Rigidbody>();
                Transform playerTransform = player.GetComponent<Transform>();
                PlayerController p = player.GetComponent<PlayerController>();
                playerBody.linearVelocity = Vector3.zero;
                playerTransform.position = initialSpawnPoints[p.ID].transform.position;
                playerTransform.rotation = Quaternion.LookRotation(-initialSpawnPoints[p.ID].transform.position);
            }

            if (scoreTeam1 >= scoreToWin || scoreTeam2 >= scoreToWin)
            {
                if (scoreTeam1 > scoreTeam2)
                {
                    OnWin(1);
                }
                else if (scoreTeam2 > scoreTeam1)
                {
                    OnWin(2);
                }
            }
        }

        public void Update()
        {
            if (GameManager.Instance._hasMinigameEnded || overtimeActive)
            {
                return;
            }


            timer -= Time.deltaTime;

            if (timer < 0f && !GameManager.Instance._hasMinigameEnded)
            {
                if (scoreTeam1 == scoreTeam2)
                {
                    overtimeActive = true;
                    ResetRound();
                }
                else
                {
                    OnWin(scoreTeam1 > scoreTeam2 ? 1 : 2);
                }
            }
        }

        private void OnWin(int winningTeam)
        {
            if (winningTeam == 1)
            {
                GameObject[] team1Players = _playerManager.Teams[0].Players.ToArray();
                object[] data = team1Players.Cast<object>().ToArray();
                onEndMinigame.Raise(this, data);
            }
            else if (winningTeam == 2)
            {
                GameObject[] team2Players = _playerManager.Teams[1].Players.ToArray();
                object[] data = team2Players.Cast<object>().ToArray();
                onEndMinigame.Raise(this, data);
            }
        }
    }
}