using System;
using System.Collections.Generic;
using System.Linq;
using EventSystem;
using Minigames;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace MinigameFramework
{
    public class GameManager : MonoBehaviour
    {
        [NonSerialized]
        public static GameManager Instance;
        private GameConfig _gameConfig;
        [NonSerialized]
        public PlayerManager PlayerManager;

        [NonSerialized]
        private float _countdownTime;
    
        [Header("Game manager config")]
        [SerializeField]
        private List<Minigame> minigames;
        [NonSerialized]
        public Minigame CurrentMinigame;
        [SerializeField]
        private SceneField lobbyScene;
        [NonSerialized]
        public bool IsMinigameSceneActive;
        private bool _isMinigameActive;
        private bool _hasMinigameEnded;

        [Header("Game Events")] 
        public GameEvent onLoadMinigame;
        public GameEvent onStartMinigame;
        public GameEvent onEnableJoining;
        public GameEvent onDisablePlayerMovement;
        public GameEvent onEnablePlayerMovement;
        
        [Header("UI Events")]
        public GameEvent onUpdateCountdownTimer;
        public GameEvent onEnableCountdownUI;
        public GameEvent onDisableCountdownUI;
        public GameEvent onEnableWinUI;
        public GameEvent onDisableWinUI;
        public GameEvent onUpdateWinUI;
        
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Start()
        {
            PlayerManager = Instance.GetComponentInChildren<PlayerManager>();
            _gameConfig = Instance.GetComponent<GameConfig>();
        }

        public void Update()
        {
            if (IsMinigameSceneActive)
            {
                CheckCountdownState();
            }
        }

        private void CheckCountdownState()
        {
            if (Instance._countdownTime > 0)
            {
                Instance._countdownTime -= Time.deltaTime;
                onUpdateCountdownTimer.Raise(this, _countdownTime.ToString("0"));
            }
            else if (!Instance._isMinigameActive && !Instance._hasMinigameEnded)
            {
                onEnablePlayerMovement.Raise(this);
                onDisableCountdownUI.Raise(this);
                onStartMinigame.Raise(this);
                Instance._isMinigameActive = true;
            }
        }

        public void OnLoadMinigame()
        {
            if (Instance.PlayerManager.Players.Count < 2) return;
            Minigame selectedMinigame;
            int currentPlayerCount = Instance.PlayerManager.Players.Count;
            List<Minigame> validMinigames = minigames.Where(m=> m.intendedPlayerCounts.Contains(currentPlayerCount)).ToList();
            do
            {
                int rnd = Random.Range(0, validMinigames.Count);
                
                selectedMinigame = validMinigames[rnd];
            } while (!selectedMinigame || selectedMinigame == Instance.CurrentMinigame && validMinigames.Count != 1);
        
            Instance.CurrentMinigame = selectedMinigame;
          
            onDisablePlayerMovement.Raise(this);
        
            onEnableCountdownUI.Raise(this);
            Instance._hasMinigameEnded = false;
            Instance._countdownTime = Instance._gameConfig.countdownLength;

            Instance.LoadScene(selectedMinigame.sceneToLoad);
            Instance.IsMinigameSceneActive = true;
        }

        public void ToLobby()
        {
            Instance.LoadScene(lobbyScene);
            Instance.IsMinigameSceneActive = false;
            Instance._isMinigameActive = false;
            onEnableJoining.Raise(this, null);
        }
    
        public void StartGame(InputAction.CallbackContext context)
        {
            if (!context.started || Instance._isMinigameActive && !Instance._hasMinigameEnded || Instance._countdownTime > 0) return;
        
            if (!Instance.IsMinigameSceneActive)
            {
                onLoadMinigame.Raise(this, null);
            }
            else
            {
                Instance.ToLobby();
            }
        }
    
        private void LoadScene(string sceneName)
        {
            onDisableWinUI.Raise(this);
            Instance.PlayerManager.ResetAllPlayerStates();
            SceneManager.LoadScene(sceneName);
            Time.timeScale = 1f;
        }

        public void ReloadScene(InputAction.CallbackContext context)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
            Time.timeScale = 1f;
        }

        public void OnEndMinigame(Component sender, object[] winnerObj)
        {
            if (winnerObj.First() is not GameObject winner) return;
            
            Time.timeScale = 0;
            string winningPlayer = winner.name;
            onEnableWinUI.Raise(this);
            onUpdateWinUI.Raise(this, winningPlayer);

            Instance._hasMinigameEnded = true;
            Instance._isMinigameActive = false;
        }
    }
}