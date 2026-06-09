using System;
using System.Collections.Generic;
using System.Linq;
using EventSystem;
using MinigameFramework;
using MinigameFramework.UIElements;
using Minigames;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ScoreboardManager : MonoBehaviour
{
    [NonSerialized] public List<int> PlayerScores = new();
    public List<GameObject> PlayerScoreTexts = new();
    public int ScoreToWin = 3;

    public GameObject elements;
    public GameObject crownUI;
    public float outwardDistance;
    private object[] winners;
    private bool isActive;

    public Minigame TieBreakerMinigame;

    //Event
    public GameEvent onUpdateWinUIText;
    public GameEvent onWinGame;
    public GameEvent onPlayerLeaveLobby;
    public GameEvent onLoadMinigame;

    private List<GameObject> _instantiatedCrowns = new List<GameObject>();

    private void Awake()
    {
        ResetPlayerScores();
    }

    public void ResetPlayerScores()
    {
        PlayerScores = new List<int> { 0, 0, 0, 0 };
    }

    public void GainPoint(Component sender, object[] winnerObjs)
    {
        if (winnerObjs == null || winnerObjs.Length == 0) return;
        foreach (GameObject player in winnerObjs)
        {
            PlayerController p = player.GetComponent<PlayerController>();
            PlayerScores[p.ID]++;
            GameObject crown = Instantiate(crownUI, elements.transform);
            crown.transform.rotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward);
            crown.transform.position += crown.transform.up * outwardDistance;
            ScoreboardCrown c = crown.GetComponent<ScoreboardCrown>();
            c.target = PlayerScoreTexts[p.ID].gameObject;
            _instantiatedCrowns.Add(crown);
        }
    }

    public void OnCleanScoreboardUI(Component sender, object[] data)
    {
        if (!isActive)
        {
            return;
        }
        isActive = false;
        
        foreach (GameObject crown in _instantiatedCrowns)
        {
            Destroy(crown);
        }
        _instantiatedCrowns.Clear();

        List<GameObject> winningPlayers = new();
        var highestScore = ScoreToWin;
        foreach (GameObject player in GameManager.Instance.PlayerManager.ActivePlayers)
        {
            PlayerController p = player.GetComponent<PlayerController>();
            if (PlayerScores[p.ID] == highestScore)
            {
                winningPlayers.Add(player);
            } else if (PlayerScores[p.ID] > highestScore)
            {
                highestScore = PlayerScores[p.ID];
                winningPlayers = new List<GameObject> { player };
            }
        }

        if (winningPlayers.Count > 1)
        {
            GameManager.Instance.CanLeaveOverride = true;
            foreach (GameObject player in GameManager.Instance.PlayerManager.ActivePlayers.Where(p => !winningPlayers.Contains(p)).ToArray())
            {
                onPlayerLeaveLobby.Raise(player.GetComponent<PlayerController>(), null);
            }
            GameManager.Instance.CanLeaveOverride = false;

            onLoadMinigame.Raise(this, TieBreakerMinigame);
        }
        else if (winningPlayers.Count == 1)
        {
            onWinGame.Raise(this, winningPlayers.Cast<object>().ToArray());
            ResetPlayerScores();
        }
        else
        {
            GameManager.Instance.ToLobby();
        }
    }

    public void OnShowCrowns(Component sender, object[] modelsObj)
    {
        foreach (GameObject crown in _instantiatedCrowns)
        {
            crown.SetActive(true);
        }
    }

    public void OnUpdateWinnerUIText(Component sender, object[] winnerDataObj)
    {
        if (winnerDataObj[0] is string && winnerDataObj[1] is Color)
        {
            winners = winnerDataObj;
        }
    }

    public void ReCallUpdateWinner()
    {
        onUpdateWinUIText.Raise(this, winners);
    }

    public void SetIsActive(bool active)
    {
        isActive = active;
    }
}
