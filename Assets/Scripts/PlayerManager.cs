using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [NonSerialized]
    public List<GameObject> Players;
    public List<Material> playerMaterials;
    
    [NonSerialized]
    public Dictionary<string, int> PlayerLives = new();
    [NonSerialized]
    public List<GameObject> AlivePlayers;
    private GameConfig _gameConfig;
    
    public void RegisterPlayer(PlayerInput player)
    {
        if (Players == null)
        {
            Players = new List<GameObject>();
        }
        Players.Add(player.gameObject);
        DontDestroyOnLoad(player.gameObject);
        player.gameObject.name = "Player" + Players.Count;
        player.gameObject.GetComponentInChildren<Renderer>().material = playerMaterials[Players.Count - 1];
    }
    
    public void Start()
    {
        Players = GameObject.FindGameObjectsWithTag("Player").ToList();
        _gameConfig = gameObject.GetComponent<GameConfig>();
    }

    public void SetPlayerLives()
    {
        AlivePlayers = Players;
        foreach (var player in Players)
        {
            player.SetActive(true);
            PlayerLives[player.name] = _gameConfig.playerLives;
        }
    }

    public void DeductLife(GameObject player)
    {
        PlayerLives[player.name]--;
        if (PlayerLives[player.name] == 0)
        {
            AlivePlayers.Remove(player);
            player.SetActive(false);
        }
    }

    public int GetAlivePlayerAmount()
    {
        int alivePlayers = AlivePlayers.Count;
        return alivePlayers;
    }
}

