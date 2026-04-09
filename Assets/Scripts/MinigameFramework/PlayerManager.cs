using System;
using System.Collections.Generic;
using System.Linq;
using EventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MinigameFramework
{
    public class PlayerManager : MonoBehaviour
    {
        [NonSerialized] public List<GameObject> Players;
        [NonSerialized] public List<GameObject> ActivePlayers;
        public List<Material> playerMaterials;
        public List<GameObject> playerModels;
        [Tooltip("The point to place the inactive players (within the deathbox)")]
        public GameObject inactivePlayerPoint;

        [NonSerialized] public bool CanJoin = false;

        [NonSerialized] public Dictionary<string, int> PlayerLives = new();
        [NonSerialized] public List<GameObject> AlivePlayers;

        public GameEvent OnEnablePlayerUI;
        public GameEvent OnDisablePlayerUI;

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
            var playerController = player.GetComponent<PlayerController>();
            playerController.id = Players.Count - 1;

            player.gameObject.name = "Player" + Players.Count;
            var model = Instantiate(playerModels[Players.Count - 1]);
            // model.GetComponentInChildren<SkinnedMeshRenderer>().materials[1] = playerMaterials[Players.Count - 1];
            model.transform.parent = player.gameObject.transform;
            player.transform.position = inactivePlayerPoint.transform.position;
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
                player.transform.position = inactivePlayerPoint.transform.position;

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

        public void OnPlayerJoinLobby(Component sender, object[] _objects)
        {
            if (!CanJoin) return;
            PlayerController player = sender.GetComponent<PlayerController>();
            player.SetPlayerState(PlayerState.Idle);
            player.IsActive = true;
            ActivePlayers.Add(sender.gameObject);

            OnEnablePlayerUI.Raise(this, player.id);
        }

        public void OnPlayerLeaveLobby(Component sender, object[] _objects)
        {
            if (!CanJoin) return;
            sender.gameObject.transform.position = inactivePlayerPoint.transform.position;
            PlayerController player = sender.GetComponent<PlayerController>();
            player.SetPlayerState(PlayerState.InputRestricted);
            player.IsActive = false;
            ActivePlayers.Remove(sender.gameObject);

            OnDisablePlayerUI.Raise(this, player.id);
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

