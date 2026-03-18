using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerManager : MonoBehaviour
{
    [NonSerialized] public List<GameObject> Players;
    public List<Material> playerMaterials;

    [NonSerialized] public Dictionary<string, int> PlayerLives = new();
    [NonSerialized] public List<GameObject> AlivePlayers;

    [FormerlySerializedAs("_gameConfig")] [SerializeField]
    private GameConfig gameConfig;
    
    public void RegisterPlayer(PlayerInput player)
    {
        if (SceneManager.GetActiveScene().name != "Main") return;
        if (Players == null || Players.Count == 0)
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
    }
    
    public void SetPlayerLives()
    {
        AlivePlayers = new List<GameObject>();
        foreach (var player in Players)
        {
            AlivePlayers.Add(player);
            player.SetActive(true);
            PlayerLives[player.name] = gameConfig.playerLives;
        }
    }

    public void ResetPlayerStates()
    {
        foreach (var player in Players)
        {
            player.GetComponent<PlayerController>().SetPlayerState(PlayerState.Idle);
            player.transform.position = new Vector3(0, 3, 0);
        }
    }

public void DeductLife(GameObject player)
    {
        PlayerLives[player.name]--;
        if (PlayerLives[player.name] == 0)
        {
            AlivePlayers.Remove(player);
            PlayerController controller  = player.GetComponent<PlayerController>();
            controller.SetPlayerState(PlayerState.InputRestricted);
        }
    }

    public int GetAlivePlayerAmount()
    {
        int alivePlayers = AlivePlayers.Count;
        return alivePlayers;
    }
}

