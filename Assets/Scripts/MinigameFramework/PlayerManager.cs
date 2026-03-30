using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MinigameFramework
{
    public class PlayerManager : MonoBehaviour
    {
        [NonSerialized] public List<GameObject> Players;
        [NonSerialized] public List<GameObject> ActivePlayers;
        public List<Material> playerMaterials;
        public GameObject deathPoint;

        [NonSerialized] public Dictionary<string, int> PlayerLives = new();
        [NonSerialized] public List<GameObject> AlivePlayers;

        [SerializeField]
        private GameConfig gameConfig;
    
        public void RegisterPlayer(PlayerInput player)
        {
            if(GameManager.Instance.IsMinigameSceneActive) return;
     
            if (Players == null || Players.Count == 0)
            {
                Players = new List<GameObject>();
                ActivePlayers = new List<GameObject>();
            }

            Players.Add(player.gameObject);
            DontDestroyOnLoad(player.gameObject);
            player.gameObject.name = "Player" + Players.Count;
            player.gameObject.GetComponentInChildren<Renderer>().material = playerMaterials[Players.Count - 1];
            player.transform.position = deathPoint.transform.position;
        }
    
        public void SetPlayerLives(Component sender, object[] livesObj)
        {
            if (livesObj.First() is not int lives) return;
            
            AlivePlayers = new List<GameObject>();
            foreach (var player in ActivePlayers)
            {
                AlivePlayers.Add(player);
                player.SetActive(true);
                PlayerLives[player.name] = lives;
            }
        }

        public void ResetPlayerStates()
        {
            foreach (var player in Players)
            {
                player.GetComponent<PlayerController>().SetPlayerState(PlayerState.InputRestricted);
                player.transform.position = deathPoint.transform.position;

                PlayerInput playerInput = player.GetComponent<PlayerInput>();
                playerInput.defaultActionMap = "Player";
                playerInput.enabled = false;
                playerInput.enabled = true;
            }
        }

        public void ResetPlayers()
        {
            foreach (var player in Players)
            {
                Destroy(player.gameObject);
            }

            Players.Clear();
        }

        public void ResetAllPlayerStates()
        {
            foreach (GameObject player in ActivePlayers)
            {
                player.transform.position = new Vector3(0, 3, 0);
            }
        }

        public void OnPlayerJoinLobby(Component sender, object[] livesObj){
            sender.gameObject.transform.position = new Vector3(0, 3, 0);
            sender.GetComponent<PlayerController>().SetPlayerState(PlayerState.Idle);
            ActivePlayers.Add(sender.gameObject);
            Debug.Log("Joining with " + sender.name);
        }

        public void OnPlayerLeaveLobby(Component sender, object[] livesObj)
        {
            sender.gameObject.transform.position = deathPoint.transform.position;
            sender.GetComponent<PlayerController>().SetPlayerState(PlayerState.InputRestricted);
            ActivePlayers.Remove(sender.gameObject);
            Debug.Log("Leaving with " + sender.name);
        }

        public void OnEnableAllPlayerMovement()
        {
            foreach (GameObject player in ActivePlayers)
            {
                PlayerController controller  = player.GetComponent<PlayerController>();
                controller.SetPlayerState(PlayerState.Idle);
                PlayerInput playerInput = player.GetComponent<PlayerInput>();
                playerInput.enabled = false;
                playerInput.enabled = true;
            }
        }

        public void OnDisableAllPlayerMovement()
        {
            foreach (GameObject player in ActivePlayers)
            {
                PlayerController controller  = player.GetComponent<PlayerController>();
                controller.SetPlayerState(PlayerState.InputRestricted);
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
}

