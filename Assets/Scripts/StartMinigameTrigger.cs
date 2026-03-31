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

        if (GameManager.Instance.PlayerManager.ActivePlayers.Count == areInside.Count 
            && !GameManager.Instance.IsMinigameSceneActive 
            && GameManager.Instance.PlayerManager.ActivePlayers.Count >= 2)
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
