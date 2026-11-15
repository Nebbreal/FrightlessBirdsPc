using System;
using System.Collections.Generic;
using System.Linq;
using EventSystem;
using Minigames;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace MinigameFramework
{
    public class PlayerManager : MonoBehaviour
    {
        private bool[] _isPlayerIdAvailable = { true, true, true, true };
        [NonSerialized] 
        public List<GameObject> Players;
        [NonSerialized] 
        public List<Rigidbody> PlayerRigidbodies = new List<Rigidbody>();
        [NonSerialized] 
        public List<GameObject> ActivePlayers = new List<GameObject>();
        [NonSerialized]
        public List<GameObject> WaitingPlayers = new List<GameObject>(); //Primarily used for when players need to be gone for 1 minigame only.
        [NonSerialized] 
        public List<GameObject> DebugPlayers;
        [NonSerialized] 
        public Dictionary<string, int> PlayerLives = new();
        [NonSerialized] 
        public List<GameObject> AlivePlayers;
        public bool playersCanJoin = false;
        
        public List<Team> Teams;
        public List<GameObject> playerModels;
        [Tooltip("The point to place the inactive players (within the deathbox)")]
        public GameObject inactivePlayerPoint;
        
        [SerializeField] 
        private string playerActionMapName = "Player";
        [SerializeField]
        private GameConfig gameConfig;
        [SerializeField]
        private GameObject playerPrefab;

        [Header("GameEvents")]
        public GameEvent onEnablePlayerUI;
        public GameEvent onDisablePlayerUI;
        public GameEvent onPlayerJoinLobby;
        public GameEvent onPlayerLeaveLobby;
        public GameEvent onRegisterPlayer;
        public GameEvent onDeRegisterPlayer;
        public GameEvent onSpawnPlayerInLobby;
        
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
        public void Join(PlayerInput player)
        {
            if (!playersCanJoin) return;
            player.transform.position = inactivePlayerPoint.transform.position;
        }
        
        public void OnRegisterPlayer(Component sender, object[] playerObj)
        {
            if (playerObj.First() is not GameObject playerObject) return;
            
            bool isDebugPlayer = false;
            if (playerObj.Length == 2 && playerObj[1] is bool)
            {
                isDebugPlayer = (bool)playerObj[1];
            }
            
            if(GameManager.Instance.IsMinigameSceneActive) return;
     
            if (Players == null || Players.Count == 0)
            {
                Players = new List<GameObject>();
                ActivePlayers = new List<GameObject>();
                DebugPlayers = new List<GameObject>();
                WaitingPlayers = new List<GameObject>();
            }
            
            if (Players != null && Players.Contains(playerObject))
            {
                return;
            }
            
            Players.Add(playerObject);
            PlayerRigidbodies.Add(playerObject.GetComponent<Rigidbody>());
            
            DontDestroyOnLoad(playerObject);
            
            if (!isDebugPlayer)
            {
                PlayerInput player = playerObject.GetComponent<PlayerInput>();
            
                player.SwitchCurrentActionMap(playerActionMapName);
                player.defaultActionMap = playerActionMapName;
            }
            
            InstantiatePlayer(playerObject);
            
            OnPlayerJoinLobby(playerObject);
        }

        private void InstantiatePlayer(GameObject playerObject)
        {
            for (int i = 0; i < _isPlayerIdAvailable.Length; i++)
            {
                if (_isPlayerIdAvailable[i])
                {
                    _isPlayerIdAvailable[i] = false;
                    PlayerController  playerController = playerObject.GetComponent<PlayerController>();
                    playerController.ID = i;

                    SetPlayerColor(playerObject, i);
                    
                    playerObject.name = "Player" + (i+1);

                    InstantiatePlayerModel(playerObject, playerController);
                    
                    playerController.SetActive(true);
                    
                    break;
                }

                if (i == _isPlayerIdAvailable.Length - 1)
                {
                    Debug.LogWarning("No more player slots available!");
                }
            }
        }

        private void InstantiatePlayerModel(GameObject playerObject, PlayerController  playerController)
        {
            if (playerObject.GetComponentInChildren<PlayerModel>() == null)
            {
                GameObject instantiatedPlayer =
                    Instantiate(playerModels[playerController.ID], playerObject.transform);
                Vector3 playerModelLocalPosition = instantiatedPlayer.transform.localPosition;
                instantiatedPlayer.transform.localPosition = new Vector3(playerModelLocalPosition.x, -1, playerModelLocalPosition.z);
                
                PlayerModel playerModel = instantiatedPlayer.GetComponentInChildren<PlayerModel>();

                playerModel.ChangeOutlineColor(PlayerColors[playerController.ID]);
            }
        }
        
        private void SetPlayerColor(GameObject playerObject, int playerIndex)
        {
            PlayerCooldownUI cooldownUI = playerObject.GetComponentInChildren<PlayerCooldownUI>();
            cooldownUI.SetCooldownColor(playerIndex);
        }

        private void UnassignPlayerID(PlayerController playerController)
        {
            int idToUnassign = playerController.ID;
            playerController.ID = -1;
            _isPlayerIdAvailable[idToUnassign] = true;
        }

        public void OnDeRegisterPlayer(Component sender, object[] playerObj)
        {
            if (playerObj.First() is not GameObject playerObject) return;
            PlayerInput player = playerObject.GetComponent<PlayerInput>();
            if(GameManager.Instance.IsMinigameSceneActive) return;
          
            PlayerController playerController = player.GetComponent<PlayerController>();
            onPlayerLeaveLobby.Raise(this,  playerObj);
            UnassignPlayerID(playerController);
            
            PlayerRigidbodies.Remove(playerObject.GetComponent<Rigidbody>());
            Players.Remove(playerObject);
            ActivePlayers.Remove(playerObject);
            Destroy(playerObject);
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
                debugPlayer.GetComponentInChildren<PlayerInput>().enabled = false;
                
                onRegisterPlayer.Raise(this, debugPlayer, true);
                DebugPlayers.Add(debugPlayer);
            }
            #endif
        }

        private void OnPlayerJoinLobby(GameObject playerObject)
        {
            if (!playersCanJoin) return;
            
            playerObject.transform.position = new Vector3(0, 3, 0);
            PlayerController player = playerObject.GetComponent<PlayerController>();
            player.SetPlayerState(PlayerState.Idle);
            player.IsActive = true;
            ActivePlayers.Add(playerObject);

            onEnablePlayerUI.Raise(this, player.ID);
            onSpawnPlayerInLobby.Raise(this, playerObject);
            
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
            if (!playersCanJoin && !GameManager.Instance.CanLeaveOverride) return;
            if (playerObj.First() is not GameObject playerObject) return;
            
            playerObject.transform.position = inactivePlayerPoint.transform.position;
            PlayerController player = playerObject.GetComponent<PlayerController>();
            player.SetPlayerState(PlayerState.InputRestricted);
            player.IsActive = false;
            ActivePlayers.Remove(playerObject);

            onDisablePlayerUI.Raise(this, player.ID);
        }

        public void OnWaitPlayer(Component sender, object[] playerObj)
        {
            if (playerObj.First() is not GameObject playerObject) return;
            
            ActivePlayers.Remove(playerObject);
            WaitingPlayers.Add(playerObject);
            playerObject.transform.position = inactivePlayerPoint.transform.position;
            PlayerController player = playerObject.GetComponent<PlayerController>();
            player.SetPlayerState(PlayerState.InputRestricted);
        }

        public void OnUnWaitPlayers(Component sender, object[] _)
        {
            foreach (GameObject playerObject in WaitingPlayers.ToList())
            {
                ActivePlayers.Add(playerObject);
                WaitingPlayers.Remove(playerObject);
                PlayerController player = playerObject.GetComponent<PlayerController>();
                player.SetPlayerState(PlayerState.Idle);
            }
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
            foreach (GameObject player in ActivePlayers.ToList())
            {
                PlayerController controller  = player.GetComponent<PlayerController>();
                controller.SetPlayerState(PlayerState.Idle);
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

