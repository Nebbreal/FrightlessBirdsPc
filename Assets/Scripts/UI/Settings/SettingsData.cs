using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Scriptable Objects/SettingsData")]
public class SettingsData : ScriptableObject
{
    // public List<ScriptableObject> settingsCategories = new List<ScriptableObject>();

    #region General
    [Header("Player Settings")]
    public float DashCooldown;
    public float AttackCooldown;
    public float BlockCooldown;
    public float PlayerMovementSpeed;
    #endregion

    #region Minigames
    [Header("Active Minigame Settings")]
    public bool FFActive;
    public bool WCActive;
    public bool GGActive;
    #endregion

    #region Volume
    [Header("Volume Settings")]
    public float MasterVolume;
    public float MusicVolume;
    public float SfxVolume;
    #endregion

    #region FrozenFeathers
    [Header("Frozen Feathers Settings")]
    public float FFPlatformEndSize;
    public float FFLives;
    #endregion

    #region WorldCoop
    [Header("World Coop Settings")]
    public float WCGameLength;
    public int WCGoalsToWin;
    #endregion

    #region GoldenGoose
    [Header("Golden Goose Settings")]
    public float GGGameLength;
    public float GGSecondsPerPoint;
    public float GGPickUpDelay;
    public float GGMinBisonUptime;
    public float GGMaxBisonUptime;
    #endregion

}
