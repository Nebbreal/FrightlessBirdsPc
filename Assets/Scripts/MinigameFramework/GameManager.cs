using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using EventSystem;
using Minigames;
using Unity.VisualScripting;
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
        public GameEvent onStartMusic;
        
        [Header("Player Events")]
        public GameEvent onResetCalibration;
        public GameEvent onEnableJoining;
        public GameEvent onDisablePlayerMovement;
        public GameEvent onEnablePlayerMovement;
        public GameEvent onInitializeTeams;
        
        [Header("UI Events")]
        public GameEvent onUpdateCountdownTimer;
        public GameEvent onEnableCountdownUI;
        public GameEvent onDisableCountdownUI;
        public GameEvent onEnableWinUI;
        public GameEvent onDisableWinUI;
        public GameEvent onUpdateWinUI;
        public GameEvent onEnableLobbyUI;
        public GameEvent onDisableLobbyUI;
        
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
            #region Timers
            Instance._countdownTime -= Time.deltaTime;
            #endregion
            
            if (IsMinigameSceneActive)
            {
                CheckCountdownState();
            }
        }

        private void CheckCountdownState()
        {
            if (Instance._countdownTime > 0)
            {
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
            if (Instance.PlayerManager.ActivePlayers.Count < 2) return;
            Minigame selectedMinigame;
            int currentPlayerCount = Instance.PlayerManager.ActivePlayers.Count;
            List<Minigame> validMinigames = minigames.Where(m=> m.intendedPlayerCounts.Contains(currentPlayerCount)).ToList();
            do
            {
                int rnd = Random.Range(0, validMinigames.Count);
                
                selectedMinigame = validMinigames[rnd];
            } while (!selectedMinigame || selectedMinigame == Instance.CurrentMinigame && validMinigames.Count != 1);
        
            Instance.CurrentMinigame = selectedMinigame;
          
            onDisablePlayerMovement.Raise(this);
        
            onInitializeTeams.Raise(this, Instance.CurrentMinigame);
            onEnableCountdownUI.Raise(this);
            Instance._hasMinigameEnded = false;
            Instance._countdownTime = Instance._gameConfig.countdownLength;

            Instance.LoadScene(selectedMinigame.sceneToLoad);
            Instance.IsMinigameSceneActive = true;
            onDisableLobbyUI.Raise(this);
            Instance.PlayerManager.CanJoin = false;
            onStartMusic.Raise(this);
        }

        private void InitTeams()
        {
            
        }

        public void ToLobby()
        {
            Instance.LoadScene(lobbyScene);
            Instance.IsMinigameSceneActive = false;
            Instance._isMinigameActive = false;
            onEnablePlayerMovement.Raise(this);
            onEnableLobbyUI.Raise(this);
            Instance.PlayerManager.CanJoin = true;
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
                Instance._countdownTime = 1; //Debounce ToLobby
            }
        }
    
        private void LoadScene(string sceneName)
        {
            onDisableWinUI.Raise(this);
            Instance.PlayerManager.ResetAllPlayerPositions();
            SceneManager.LoadScene(sceneName);
            Time.timeScale = 1f;
        }

        public void ReloadScene(InputAction.CallbackContext context)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
            Time.timeScale = 1f;
        }

        public void OnEndMinigame(Component sender, object[] winnerObjs)
        {
            if (winnerObjs == null || winnerObjs.Length == 0) return;

            Time.timeScale = 0;

            List<string> winnerNames = new List<string>();

            foreach (var obj in winnerObjs)
            {
                if (obj is GameObject playerGo)
                {
                    winnerNames.Add(playerGo.name);
                }
                else if (obj is string teamName)
                {
                    winnerNames.Add(teamName); 
                }
            }
            string winningDisplayString = string.Join(", ", winnerNames);

            onEnableWinUI.Raise(this);
            onUpdateWinUI.Raise(this, winningDisplayString);

            Instance._hasMinigameEnded = true;
            Instance._isMinigameActive = false;
        }

        public void ResetCalibration(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Instance.PlayerManager.ResetPlayers();
                onResetCalibration.Raise(this);
            }
        }

        public void CompleteCalibration(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (Instance.PlayerManager.GetComponent<CalibrationManager>().ConfirmCalibration())
                {
                    //To make sure there are no issues with the player input manager detecting a new PlayerInput, we have to find it and turn it off.
                    PlayerInputManager playerInputManager = FindAnyObjectByType<PlayerInputManager>();
                    playerInputManager.enabled = false;
                    Instance.PlayerManager.ResetPlayerStates();
                    onEnableLobbyUI.Raise(this);
                    Instance.PlayerManager.CanJoin = true;
                }
                
            }
        }
    }
}