using System.Collections.Generic;
using UnityEngine;
using EventSystem;
using MinigameFramework;

public class StartMinigameTrigger : MonoBehaviour
{
    public GameEvent onStartMinigame;
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
        areInside.Add(other.gameObject);

        GameManager gameManager = GameManager.Instance;
        PlayerManager playerManager = gameManager.PlayerManager;

        if (playerManager.ActivePlayers.Count == areInside.Count 
            && !gameManager.IsMinigameSceneActive 
            && playerManager.ActivePlayers.Count >= 2)
        {
            onStartMinigame.Raise(this, other.gameObject);
            areInside = new List<GameObject>();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        areInside.Remove(other.gameObject);
    }
}
