using System;
using System.Collections.Generic;
using System.Linq;
using EventSystem;
using Minigames;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace MinigameFramework
{
    public class PlayerManager : MonoBehaviour
    {
        [NonSerialized] public List<GameObject> Players;
        [NonSerialized] public List<GameObject> ActivePlayers;
        [NonSerialized] public List<GameObject> DebugPlayers;
        public List<Team> Teams;
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
        [SerializeField]
        private GameObject playerPrefab;

        #region PlayerInit
        public void RegisterPlayer(PlayerInput player)
        {
            if(GameManager.Instance.IsMinigameSceneActive) return;
     
            if (Players == null || Players.Count == 0)
            {
                Players = new List<GameObject>();
                ActivePlayers = new List<GameObject>();
                DebugPlayers = new List<GameObject>();
            }

            Players.Add(player.gameObject);
            DontDestroyOnLoad(player.gameObject);
            var playerController = player.GetComponent<PlayerController>();
            playerController.id = Players.Count - 1;

            player.gameObject.name = "Player" + Players.Count;
            Instantiate(playerModels[Players.Count - 1], player.gameObject.transform, true);
            // model.GetComponentInChildren<SkinnedMeshRenderer>().materials[1] = playerMaterials[Players.Count - 1];
            player.transform.position = inactivePlayerPoint.transform.position;
        }
        
        public void OnRegisterDebugPlayers (Component sender, object[] objects)
        {
            #if  UNITY_EDITOR
            if (DebugPlayers == null || DebugPlayers.Count == 0)
            {
                DebugPlayers = new List<GameObject>();
            }
            
            if(Players.Count == 0) return;
           
            int missingPlayerCount = 4 - Players.Count;
            for (int i = missingPlayerCount; i > 0; i--)
            {
                GameObject debugPlayer = Instantiate(playerPrefab);
                DebugPlayers.Add(debugPlayer);
            }
            #endif
        }
        
        public void OnPlayerJoinLobby(Component sender, object[] objects)
        {
            if (!CanJoin) return;
            sender.gameObject.transform.position = new Vector3(0, 3, 0);
            PlayerController player = sender.GetComponent<PlayerController>();
            player.SetPlayerState(PlayerState.Idle);
            player.IsActive = true;
            ActivePlayers.Add(sender.gameObject);

            OnEnablePlayerUI.Raise(this, player.id);

            #if UNITY_EDITOR
            //Disable input of initiated debug players
            GameObject playerObj = player.gameObject;
            if (DebugPlayers.Contains(playerObj))
            {
                playerObj.GetComponent<PlayerInput>().enabled = false;
            }
            #endif
        }

        public void OnPlayerLeaveLobby(Component sender, object[] objects)
        {
            if (!CanJoin) return;
            sender.gameObject.transform.position = inactivePlayerPoint.transform.position;
            PlayerController player = sender.GetComponent<PlayerController>();
            player.SetPlayerState(PlayerState.InputRestricted);
            player.IsActive = false;
            ActivePlayers.Remove(sender.gameObject);

            OnDisablePlayerUI.Raise(this, player.id);
        }
        #endregion

        #region Teams
        public void OnInitializeTeams(Component sender, object[] minigameConfigObj)
        {
            Teams = new List<Team>();
            if (minigameConfigObj.First() is not Minigame minigameConfig) return;
            List<int> teamSizes = minigameConfig.teamSizes;
            
            if (teamSizes.Count == 0) return;
            foreach (int teamSize in teamSizes)
            {
                Team teamToAdd = new Team(teamSize);
                Teams.Add(teamToAdd);
            }

            int requiredPlayerCount = Teams.Sum(team => team.Size);
            if (requiredPlayerCount > ActivePlayers.Count)
            {
                Debug.LogError("Not enough players to fill the teams! Required: " + requiredPlayerCount + ", Available: " + ActivePlayers.Count);
                return;
            }
            
            List<GameObject> unassignedPlayers = new List<GameObject>(ActivePlayers);
            
            if (Teams.All(team => team.Size == 2)) //2v2
            {
                Init2Versus2Teams(unassignedPlayers);
            }
            else if (Teams.Count == 2 && Teams.Any(team => team.Size is 3 or 2)) //3v1 or 2v1 or 2v1v1
            {
                InitAsymmetricalTeams(unassignedPlayers, Teams.Count);
            }
            List<Team> teamsToFill = Teams.Where(team => team.Size > team.Players.Count).ToList();

            if (unassignedPlayers.Count > 0)
            {
                HandlePlayerOverflow(unassignedPlayers, teamsToFill);
            }
        }

        private void HandlePlayerOverflow(List<GameObject> unassignedPlayers, List<Team> teamsToFill)
        {
            if (teamsToFill.Count > 0)
            {
                foreach (Team team in teamsToFill)
                {
                    int missingPlayerCount = team.Size - team.Players.Count;
                    if (unassignedPlayers.Count < missingPlayerCount)
                    {
                        Debug.LogError("Not enough players to fill the teams!");
                    }
                    else
                    {
                        unassignedPlayers.Take(missingPlayerCount).ToList().ForEach(player =>
                        {
                            team.Players.Add(player);
                            unassignedPlayers.Remove(player);
                        });
                    }
                }
            }
            else
            {
                Debug.LogError("Too many players for the team sizes! Unassigned players will be put in an overflow team.");
                Team overflowTeam = new Team(unassignedPlayers.Count)
                {
                    Players = unassignedPlayers
                };
                Teams.Add(overflowTeam);
            }
        }

        private void Init2Versus2Teams(List<GameObject> unassignedPlayers)
        {
            foreach (GameObject player in unassignedPlayers.ToList())
            {
                switch (player.name)
                {
                    case "Player1":
                    case "Player4":
                        Teams[0].Players.Add(player);
                        unassignedPlayers.Remove(player);
                        break;
                    case "Player2":
                    case "Player3": 
                        Teams[1].Players.Add(player);
                        unassignedPlayers.Remove(player);
                        break;
                }
            }
        }

        private void InitAsymmetricalTeams(List<GameObject> unassignedPlayers, int teamAmount)
        {
            for (int i = 0; i < teamAmount - 1; i++)
            {
                int rndPlayerIndex = Random.Range(0, unassignedPlayers.Count);
                Team singlePlayerTeam = Teams.First(team => team.Size == 1 && team.Players.Count == 0);
                singlePlayerTeam.Players.Add(unassignedPlayers[rndPlayerIndex]);
                unassignedPlayers.Remove(unassignedPlayers[rndPlayerIndex]);
            }
            
            Team multiplePlayersTeam = Teams.Find(team => team.Size != 1);
            multiplePlayersTeam.Players.AddRange(unassignedPlayers);
        }
        #endregion
        
        #region PlayerStates
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

        public void ResetAllPlayerPositions()
        {
            foreach (GameObject player in ActivePlayers)
            {
                player.transform.position = new Vector3(0, 3, 0);
            }
        }

        public void OnEnableAllPlayerMovement()
        {
            foreach (GameObject player in ActivePlayers)
            {
                PlayerController controller  = player.GetComponent<PlayerController>();
                controller.SetPlayerState(PlayerState.Idle);
                
                PlayerInput playerInput = player.GetComponent<PlayerInput>();
                playerInput.enabled = false;
                if (!DebugPlayers.Contains(player))
                {
                    playerInput.enabled = true;

                }
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
        

        #endregion
        
        #region PlayerLives
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
        #endregion
    }
}

