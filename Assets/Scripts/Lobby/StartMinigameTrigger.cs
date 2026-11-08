using System.Collections.Generic;
using UnityEngine;
using EventSystem;
using MinigameFramework;
using UnityEngine.Serialization;

public class StartMinigameTrigger : MonoBehaviour
{
    public GameEvent onLoadMinigame;
    private List<GameObject> areInside;

    public void Start()
    {
        if (areInside == null)
        {
            areInside = new List<GameObject>();
        }
    }

    public void Awake()
    {
        if (areInside == null)
        {
            areInside = new List<GameObject>();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        PlayerUI p = other.GetComponentInChildren<PlayerUI>();
        if (!p) return;
        areInside.Add(other.gameObject);
        p.IsReady();

        GameManager gameManager = GameManager.Instance;
        PlayerManager playerManager = gameManager.PlayerManager;

        if (playerManager.ActivePlayers.Count == areInside.Count 
            && !gameManager.IsMinigameSceneActive 
            && playerManager.ActivePlayers.Count >= 2)
        {
            onLoadMinigame.Raise(this, other.gameObject);
            areInside = new List<GameObject>();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        PlayerUI p = other.GetComponentInChildren<PlayerUI>();
        if (!p) return;
        areInside.Remove(other.gameObject);
        p.NotReady();
    }
}
