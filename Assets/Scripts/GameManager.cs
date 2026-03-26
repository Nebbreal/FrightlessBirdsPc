using System;
using System.Collections.Generic;
using System.Linq;
using EventSystem;
using Minigames;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [NonSerialized]
    public static GameManager Instance;
    private UIManager _uiManager;
    private PlayerManager _playerManager;
    
    [Header("Game config")]
    [SerializeField]
    private List<Minigame> minigames;
    private Minigame _currentMinigame;
    [SerializeField]
    private SceneField lobbyScene;
    [NonSerialized]
    public bool IsMinigameSceneActive = false;
    [NonSerialized]
    public bool ArePlayersPlaying = false;

    [Header("Events")] 
    public GameEvent onStartMinigame;

    public GameEvent onEnableJoining;
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
        _uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        _playerManager = GetComponentInChildren<PlayerManager>();
    }

    public void PickRandomMinigame()
    {
        Minigame selectedMinigame = null;
        
        do
        {
            int rnd = Random.Range(0, minigames.Count);
            selectedMinigame = minigames[rnd];
            Debug.Log("Selected minigame: " + selectedMinigame.name);
        } while (!selectedMinigame || selectedMinigame == _currentMinigame && minigames.Count != 1);

        Instance.IsMinigameSceneActive = true;
        Instance.LoadScene(selectedMinigame.sceneToLoad);
    }

    public void ToLobby()
    {
        Instance.LoadScene(lobbyScene);
        Instance.IsMinigameSceneActive = false;
        onEnableJoining.Raise(this, null);
    }
    
    public void StartGame(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!Instance.IsMinigameSceneActive)
            {
                onStartMinigame.Raise(this, null);
            }
            else
            {
                Instance.ToLobby();
            }
        }
    }
    
    private void LoadScene(string sceneName)
    {
        Instance._uiManager.SetWinUI(false);
        Instance._playerManager.ResetPlayerStates();
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
        Instance.ArePlayersPlaying = false;
    }
}