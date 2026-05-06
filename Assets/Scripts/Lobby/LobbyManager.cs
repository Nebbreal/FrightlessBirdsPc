using System.Collections.Generic;
using MinigameFramework;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public List<GameObject> playerSpawns;

    public void OnJoinLobby(Component sender, object[] _objects)
    {
        int id = GameManager.Instance.PlayerManager.Players.IndexOf(sender.gameObject);
        playerSpawns[id].gameObject.GetComponentInChildren<Interactable>().Reset();
        sender.transform.position = playerSpawns[id].transform.position;
    }
}
