using System;
using System.Collections.Generic;
using System.Linq;
using Minigames;
using Minigames.FrozenFeathers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [NonSerialized]
    public static GameManager Instance;
    private UIManager _uiManager;
    private PlayerManager _playerManager;
    [SerializeField]
    private List<Minigame> minigames;

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
        GameObject tempobj = gameObject;
        _playerManager = GetComponentInChildren<PlayerManager>();
    }
    
    //To be reworked after first prototype test
    public void StartGame(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("Scenes/Feature/FrozenFeathers");
    }
    
    public void SwitchScene(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Instance._uiManager.SetWinUI(false);
            Instance._playerManager.ResetPlayerStates();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name == "Main"
                ? "Scenes/Feature/FrozenFeathers"
                : "Scenes/Main");
            Time.timeScale = 1f;
        }
    }
    
    public void ReloadScene(InputAction.CallbackContext context)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        Time.timeScale = 1f;
    }

    public void OnWin()
    {
        Time.timeScale = 0;
        string winningPlayer = Instance._playerManager.AlivePlayers.First().name;
        Instance._uiManager.UpdateWinningPlayer(winningPlayer);
    }
}
