using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private List<GameObject> players;
    public List<Material> playerMaterials;

    public void RegisterPlayer(PlayerInput player)
    {
        if (players == null)
        {
            players = new List<GameObject>();
        }
        players.Add(player.gameObject);
        DontDestroyOnLoad(player.gameObject);
        player.gameObject.name = "Player" + players.Count;
        player.gameObject.GetComponentInChildren<Renderer>().material = playerMaterials[players.Count - 1];
    }
}

