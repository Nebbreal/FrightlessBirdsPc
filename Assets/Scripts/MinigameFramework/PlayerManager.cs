using System;
using System.Collections.Generic;
using System.Linq;
using EventSystem;
using Minigames;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace MinigameFramework
{
    public class PlayerManager : MonoBehaviour
    {
        [NonSerialized] public List<GameObject> Players;
        [NonSerialized] public List<Rigidbody> PlayerRigidbodies = new List<Rigidbody>();
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


        [SerializeField]
        private GameConfig gameConfig;
        [SerializeField]
        private GameObject playerPrefab;

        [Header("GameEvents")]
        public GameEvent onEnablePlayerUI;
        public GameEvent onDisablePlayerUI;
        //TODO: Use a scriptable object linked to the player's to contain this data (Probably not able to be completed before handover)
        [NonSerialized] 
        public Dictionary<int, string> PlayerNames = new Dictionary<int, string>
        {
            { 0, "Blue" },
            { 1, "Red" },
            { 2, "Purple" },
            { 3, "Green" }
        };
        [NonSerialized] 
        public Dictionary<int, Color> PlayerColors = new Dictionary<int, Color>
        {
            { 0, new Color(0.08113199f, 0.8873519f, 1f)},
            { 1, new Color(1f, 0f, 0f) },
            { 2, new Color(0.4980392f, 0.3019608f, 0.6980392f) },
            { 3, new Color(0f, 1f, 0.047058f)},
        };
        
        #region PlayerInit
        public void RegisterPlayer(PlayerInput player)
        {
            if(!GameManager.Instance || GameManager.Instance.IsMinigameSceneActive || player.currentActionMap.name == "Operator") return;
     
            if (Players == null || Players.Count == 0)
            {
                Players = new List<GameObject>();
                ActivePlayers = new List<GameObject>();
                DebugPlayers = new List<GameObject>();
            }
            GameObject playerObj = player.gameObject;
            Players.Add(playerObj);
            PlayerRigidbodies.Add(playerObj.GetComponent<Rigidbody>());
            DontDestroyOnLoad(playerObj);
            var playerController = player.GetComponent<PlayerController>();
            playerController.ID = Players.Count - 1;

            player.gameObject.name = "Player" + Players.Count;
            GameObject instantiatedPlayer = Instantiate(playerModels[playerController.ID], playerObj.transform, true);
            
            PlayerModel playerModel = instantiatedPlayer.GetComponentInChildren<PlayerModel>();
            playerModel.ChangeOutlineColor(PlayerColors[playerController.ID]);
            
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
        
        public void OnPlayerJoinLobby(Component sender, object[] playerObj)
        {
            if (!CanJoin) return;
            if (playerObj.First() is not GameObject playerObject) return;
            
            playerObject.transform.position = new Vector3(0, 3, 0);
            PlayerController player = playerObject.GetComponent<PlayerController>();
            player.SetPlayerState(PlayerState.Idle);
            player.IsActive = true;
            ActivePlayers.Add(playerObject);

            onEnablePlayerUI.Raise(this, player.ID);

            #if UNITY_EDITOR
            //Disable input of initiated debug players
            if (DebugPlayers.Contains(playerObject))
            {
                playerObject.GetComponent<PlayerInput>().enabled = false;
            }
            #endif
        }

        public void OnPlayerLeaveLobby(Component sender, object[] playerObj)
        {
            if (!CanJoin && !GameManager.Instance.CanLeaveOverride) return;
            if (playerObj.First() is not GameObject playerObject) return;
            
            playerObject.transform.position = inactivePlayerPoint.transform.position;
            PlayerController player = playerObject.GetComponent<PlayerController>();
            player.SetPlayerState(PlayerState.InputRestricted);
            player.IsActive = false;
            ActivePlayers.Remove(playerObject);

            onDisablePlayerUI.Raise(this, player.ID);
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

            RestorePlayerOutlineColors();
            ApplyTeamOutlineColors(minigameConfig);
        }

        public void ClearTeams(Component sender, object[] data)
        {
            Teams.Clear();
        }

        private void ApplyTeamOutlineColors(Minigame minigameConfig)
        {
            if (minigameConfig.teamOutlineColors == null || minigameConfig.teamOutlineColors.Count == 0)
            {
                Debug.LogWarning("No team outline colors defined in the minigame config. Skipping outline color application.");
                return;
            }

            for (int teamIndex = 0; teamIndex < Teams.Count && teamIndex < minigameConfig.teamOutlineColors.Count; teamIndex++)
            {
                Color teamColor = minigameConfig.teamOutlineColors[teamIndex];
                foreach (GameObject player in Teams[teamIndex].Players)
                {
                    PlayerModel playerModel = player.GetComponentInChildren<PlayerModel>();
                    playerModel.ChangeOutlineColor(teamColor);
                }
            }
        }

        public void RestorePlayerOutlineColors()
        {
            if (Players == null)
            {
                return;
            }

            foreach (GameObject player in Players)
            {
                int playerID = player.GetComponent<PlayerController>().ID;
                PlayerModel playerModel = player.GetComponentInChildren<PlayerModel>();
                playerModel.ChangeOutlineColor(PlayerColors[playerID]);
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
                int playerID = player.GetComponent<PlayerController>().ID;
                switch (playerID)
                {
                    case 0:
                    case 3:
                        Teams[0].Players.Add(player);
                        unassignedPlayers.Remove(player);
                        break;
                    case 1:
                    case 2: 
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
            RestorePlayerOutlineColors();
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
                player.transform.position = new Vector3(0f, 3f, 0f);
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

        // Bridge method so `onRestorePlayerOutlineColors` GameEvent can call into this class
        public void OnRestorePlayerOutlineColors(Component sender, object[] data)
        {
            RestorePlayerOutlineColors();
        }

        public void ResetAllPlayersGravity()
        {
            foreach (Rigidbody playerRb in PlayerRigidbodies)
            {
                playerRb.useGravity = true;
                playerRb.linearDamping = 1f;
                playerRb.linearVelocity = Vector3.zero;
            }
        }

        public void ResetAllPlayerDeceleration()
        {
            foreach (GameObject player in Players)
            {
                PlayerController controller  = player.GetComponent<PlayerController>();
                controller.ResetDecelerationMultiplierOverride();
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

