using System;
using System.Collections.Generic;
using System.Linq;
using EventSystem;
using Minigames;
using TMPro;
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
        private UIManager _uiManager;
        private PlayerManager _playerManager;

        [NonSerialized]
        private float _countdownTime;
    
        [Header("Game manager config")]
        [SerializeField]
        private List<Minigame> minigames;
        private Minigame _previousMinigame;
        [SerializeField]
        private SceneField lobbyScene;
        [NonSerialized]
        public bool IsMinigameSceneActive;
        [NonSerialized]
        private bool _isMinigameActive;

        [Header("Events")] 
        public GameEvent onLoadMinigame;
        public GameEvent onStartMinigame;
        public GameEvent onEnableJoining;
        public GameEvent onCountdownTimerUpdate;
        public GameEvent onCountdownEnable;
        public GameEvent onCountdownDisable;
        public GameEvent onDisablePlayerMovement;
        public GameEvent onEnablePlayerMovement;
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
            _uiManager = Instance.GetComponentInChildren<UIManager>();
            _playerManager = Instance.GetComponentInChildren<PlayerManager>();
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
                onCountdownTimerUpdate.Raise(this, _countdownTime.ToString("0"));
            }
            else if (!_isMinigameActive)
            {
                onEnablePlayerMovement.Raise(this);
                onCountdownDisable.Raise(this);
                onStartMinigame.Raise(this);
                Instance._isMinigameActive = true;
            }
        }

        public void OnLoadMinigame()
        {
            Minigame selectedMinigame = null;
        
            do
            {
                int rnd = Random.Range(0, minigames.Count);
                selectedMinigame = minigames[rnd];
                Debug.Log("Selected minigame: " + selectedMinigame.name);
            } while (!selectedMinigame || selectedMinigame == Instance._previousMinigame && minigames.Count != 1);
        
            Instance._previousMinigame = selectedMinigame;
          
            onDisablePlayerMovement.Raise(this);
        
            Instance._countdownTime = Instance._gameConfig.countdownLength;
            onCountdownEnable.Raise(this);
        
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
            if (!context.started || _isMinigameActive) return;
        
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
            Instance._uiManager.SetWinUI(false);
            Instance._playerManager.ResetAllPlayerStates();
            SceneManager.LoadScene(sceneName);
            Time.timeScale = 1f;
        }

        public void ReloadScene(InputAction.CallbackContext context)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
            Time.timeScale = 1f;
        }

        public void OnEndMinigame()
        {
            Time.timeScale = 0;
            string winningPlayer = Instance._playerManager.AlivePlayers.First().name;
            Instance._uiManager.UpdateWinningPlayer(winningPlayer);
            Instance._isMinigameActive = false;
        }
    }
}