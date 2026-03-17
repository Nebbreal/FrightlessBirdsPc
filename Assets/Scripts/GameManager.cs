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

    public void ReloadScene(InputAction.CallbackContext context)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void OnWin()
    {
        string winningPlayer = _playerManager.AlivePlayers.First().name;
        _uiManager.UpdateWinningPlayer(winningPlayer);
    }
}
