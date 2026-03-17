using System;
using System.Linq;
using Minigames.FrozenFeathers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private UIManager _uiManager;
    private MockPlayerManager _playerManager;
    public void Start()
    {
        _uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        _playerManager = GetComponent<MockPlayerManager>();
    }

    //To be reworked after first prototype test
    public void StartGame(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("Scenes/Feature/FrozenFeathers");
    }
    
    public void ToMainMenu(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("Scenes/Main");
        Time.timeScale = 1f;
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
        string winningPlayer = _playerManager.AlivePlayers.First().name;
        _uiManager.UpdateWinningPlayer(winningPlayer);
    }
}
