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
        private float _winUITime;
    
        [Header("Game manager config")]
        [SerializeField]
        private List<Minigame> minigames;
        [NonSerialized]
        public Minigame CurrentMinigame;
        [SerializeField]
        private SceneField lobbyScene;
        [NonSerialized]
        public bool IsMinigameSceneActive;
        [NonSerialized]
        public bool IsMinigameActive;
        public bool _hasMinigameEnded;
        [NonSerialized]
        public bool IsWinUIActive;

        [Header("Game Events")] 
        public GameEvent onLoadMinigame;
        public GameEvent onStartMinigame;
        public GameEvent onStartMusic;
        public GameEvent onClearTeams;
        
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
        public GameEvent onUpdateWinnerUIText;
        public GameEvent onCleanWinUI;
        public GameEvent onUpdateWinUIModel;
        public GameEvent onEnableLobbyUI;
        public GameEvent onDisableLobbyUI;
        public GameEvent onEnableScoreboardUI;
        public GameEvent onRestorePlayerOutlineColors;

        [SerializeField]
        private Color defaultColor = Color.HSVToRGB(280, 49, 79);
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
            Instance._winUITime -= Time.deltaTime;
            #endregion
            
            if (IsMinigameSceneActive)
            {
                CheckCountdownState();
            }

            if (Instance.IsWinUIActive)
            {
                CheckWinUITime();
            }
        }

        private void CheckWinUITime()
        {
            if(Instance._winUITime <= 0)
            {
                onCleanWinUI.Raise(this, null);
                onDisableWinUI.Raise(this);
                onEnableScoreboardUI.Raise(this);
                Instance.IsWinUIActive = false;
            }
        }

        public void SpeedUpWinUI()
        {
            //TODO remove && "Instance._gameConfig.winScreenLength - Instance._winUITime > 0.5f" when scene transitions get added
            if (Instance._winUITime > 0 && Instance._gameConfig.winScreenLength - Instance._winUITime > 0.5f)
            {
                Instance._winUITime -= Instance._gameConfig.winScreenLength / Instance.PlayerManager.ActivePlayers.Count;
            }
        }

        private void CheckCountdownState()
        {
            if (Instance._countdownTime > 0)
            {
                onUpdateCountdownTimer.Raise(this, _countdownTime.ToString("0"));
            }
            else if (!Instance.IsMinigameActive && !Instance._hasMinigameEnded)
            {
                onEnablePlayerMovement.Raise(this);
                onDisableCountdownUI.Raise(this);
                onStartMinigame.Raise(this);
                Instance.IsMinigameActive = true;
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
            PlayerManager.ResetAllPlayersGravity();
            Instance.IsMinigameSceneActive = true;
            onDisableLobbyUI.Raise(this);
            Instance.PlayerManager.CanJoin = false;
            onStartMusic.Raise(this);
        }

        public void ToLobby()
        {
            Instance.LoadScene(lobbyScene);
            Instance.IsMinigameSceneActive = false;
            Instance.IsMinigameActive = false;
            onRestorePlayerOutlineColors.Raise(this);
            PlayerManager.ResetAllPlayersGravity();
            PlayerManager.ResetAllPlayerDeceleration();
            onEnablePlayerMovement.Raise(this);
            onEnableLobbyUI.Raise(this);
            Instance.PlayerManager.CanJoin = true;
        }
    
        public void StartGame(InputAction.CallbackContext context)
        {
            if (!context.started || Instance.IsMinigameActive && !Instance._hasMinigameEnded || Instance._countdownTime > 0) return;
        
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
            onCleanWinUI.Raise(this, null);
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

            onDisablePlayerMovement.Raise(this);
            Instance.PlayerManager.RestorePlayerOutlineColors();
            
            List<string> winnerNames = new List<string>();
            List<GameObject> winnerModels = new();
            Color winnerColor = defaultColor;
            foreach (var obj in winnerObjs)
            {
                if (obj is GameObject player)
                {
                    int playerID = player.GetComponent<PlayerController>().ID;
                    string playerName = PlayerManager.PlayerNames[playerID];
                    GameObject playerModel = player.GetComponentInChildren<PlayerModel>().gameObject;

                    if (player == PlayerManager.Teams.Any(t => t.Players.Contains(player)))
                    {
                        winnerColor = defaultColor;
                    }
                    else
                    {
                        winnerColor = Instance.PlayerManager.PlayerColors[playerID];    
                    }
                    
                    winnerModels.Add(playerModel);
                    winnerNames.Add(playerName);
                }
                else if (obj is string teamName)
                {
                    winnerNames.Add(teamName); 
                }
            }
            string winningDisplayString = string.Join(", ", winnerNames);
            object[] winnerData = { winningDisplayString, winnerColor };
            
            onEnableWinUI.Raise(this);
            onUpdateWinUIModel.Raise(this, winnerModels);
            onUpdateWinnerUIText.Raise(this, winnerData);
            Instance._winUITime = Instance._gameConfig.winScreenLength;
            Instance.IsWinUIActive = true;

            onClearTeams.Raise(this);
            Instance._hasMinigameEnded = true;
            Instance.IsMinigameActive = false;
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