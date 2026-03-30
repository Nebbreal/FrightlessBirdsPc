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
        public List<Material> playerMaterials;

        [NonSerialized] public Dictionary<string, int> PlayerLives = new();
        [NonSerialized] public List<GameObject> AlivePlayers;

        [SerializeField]
        private GameConfig gameConfig;
    
        private PlayerInputManager _playerInputManager;
        public void Start()
        {
            Players = GameObject.FindGameObjectsWithTag("Player").ToList();
            _playerInputManager = GetComponent<PlayerInputManager>();
        }
    
        public void RegisterPlayer(PlayerInput player)
        {
            if(GameManager.Instance.IsMinigameSceneActive) return;
     
            if (Players == null || Players.Count == 0)
            {
                Players = new List<GameObject>();
            }

            Players.Add(player.gameObject);
            DontDestroyOnLoad(player.gameObject);
            player.gameObject.name = "Player" + Players.Count;
            player.gameObject.GetComponentInChildren<Renderer>().material = playerMaterials[Players.Count - 1];
        }
    
        public void SetPlayerLives(Component sender, object[] livesObj)
        {
            if (livesObj.First() is not int lives) return;
            
            AlivePlayers = new List<GameObject>();
            foreach (var player in Players)
            {
                AlivePlayers.Add(player);
                player.SetActive(true);
                PlayerLives[player.name] = lives;
            }
        }

        public void ResetAllPlayerStates()
        {
            foreach (GameObject player in Players)
            {
                player.transform.position = new Vector3(0, 3, 0);
            }
        }

        public void OnEnableAllPlayerMovement()
        {
            foreach (GameObject player in Players)
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
            foreach (GameObject player in Players)
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

