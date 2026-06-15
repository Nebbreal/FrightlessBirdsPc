using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Scriptable Objects/SettingsData")]
public class SettingsData : ScriptableObject
{
    // public List<ScriptableObject> settingsCategories = new List<ScriptableObject>();

    #region General
    public float DashCooldown;
    public float AttackCooldown;
    public float BlockCooldown;
    public float PlayerMovementSpeed;
    #endregion

    #region Minigames
    public bool FFActive;
    public bool WCActive;
    public bool GGActive;
    #endregion

    #region Volume
    public float MasterVolume;
    public float MusicVolume;
    public float SfxVolume;
    #endregion

    #region FrozenFeathers
    public float FFPlatformEndSize;
    public float FFLives;
    #endregion

    #region WorldCoop
    public float WCGameLength;
    public int WCGoalsToWin;
    #endregion

    #region GoldenGoose
    public float GGGameLength;
    public float GGSecondsPerPoint;
    public float GGPickUpDelay;
    public float GGMinBisonUptime;
    public float GGMaxBisonUptime;
    #endregion

}
