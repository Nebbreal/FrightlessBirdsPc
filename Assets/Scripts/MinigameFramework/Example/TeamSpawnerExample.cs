using System;
using System.Collections.Generic;
using MinigameFramework;
using UnityEngine;

public class TeamSpawnerExample : MonoBehaviour
{
    [SerializeField] 
    private List<GameObject> initialSpawnPoints;
    
    public void Start()
    {
        List<Team> teams = GameManager.Instance.PlayerManager.Teams;
        
        if  (teams.Count == 0 ||  teams.Count > 2) GameManager.Instance.ToLobby();
        for (int i = 0 ; i < teams.Count; i++)
        {
            Team team = teams[i];

            foreach (GameObject player in team.Players)
            {
                Rigidbody playerBody = player.GetComponent<Rigidbody>();
                Transform playerTransform = player.GetComponent<Transform>();
                playerBody.linearVelocity = Vector3.zero;
                playerTransform.position = initialSpawnPoints[i].transform.position;
                playerTransform.rotation = Quaternion.LookRotation(-initialSpawnPoints[i].transform.position);
            }
        }
    }
}
