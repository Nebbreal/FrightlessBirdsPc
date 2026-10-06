using System.Collections.Generic;
using System.Linq;
using MinigameFramework;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public List<GameObject> playerSpawns;

    public void OnJoinLobby(Component sender, object[] _objects)
    {
        if (_objects.First() is not GameObject player) return;
        
        int id = player.GetComponent<PlayerController>().ID;
        playerSpawns[id].gameObject.GetComponentInChildren<Interactable>().Reset();
        player.transform.position = playerSpawns[id].transform.position;
    }

    public void Start()
    {
        List<GameObject> activePlayers = GameManager.Instance.PlayerManager.ActivePlayers;
        if (activePlayers.Count == 0) return;
        
        foreach (GameObject player in activePlayers)
        {
            PlayerController p = player.GetComponent<PlayerController>();
            if (p.IsActive)
            {
                player.transform.position = playerSpawns[p.ID].transform.position;
            }
        }
    }
}
