using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Scriptable Objects/SettingsData")]
public class SettingsData : ScriptableObject
{
    // public List<ScriptableObject> settingsCategories = new List<ScriptableObject>();

    #region General
    [FormerlySerializedAs("DashCooldown")] [Header("Player Settings")]
    public float dashCooldown;
    [FormerlySerializedAs("AttackCooldown")] public float attackCooldown;
    [FormerlySerializedAs("BlockCooldown")] public float blockCooldown;
    [FormerlySerializedAs("PlayerMovementSpeed")] public float playerMovementSpeed;
    #endregion

    #region Minigames
    [FormerlySerializedAs("FFActive")] [Header("Active Minigame Settings")]
    public bool frozenFeathersActive;
    [FormerlySerializedAs("WCActive")] public bool worldCoopActive;
    [FormerlySerializedAs("GGActive")] public bool goldenGooseActive;
    #endregion

    #region Volume
    [FormerlySerializedAs("MasterVolume")] [Header("Volume Settings")]
    public float masterVolume;
    [FormerlySerializedAs("MusicVolume")] public float musicVolume;
    [FormerlySerializedAs("SfxVolume")] public float sfxVolume;
    #endregion

    #region FrozenFeathers
    [FormerlySerializedAs("FFPlatformEndSize")] [Header("Frozen Feathers Settings")]
    public float frozenFeathersPlatformEndSize;
    [FormerlySerializedAs("FFLives")] public float frozenFeathersLives;
    [FormerlySerializedAs("FFPlatformShrinkTime")] public float frozenFeathersPlatformShrinkTime;
    #endregion

    #region WorldCoop
    [FormerlySerializedAs("WCGameLength")] [Header("World Coop Settings")]
    public float worldCoopGameLength;
    [FormerlySerializedAs("WCGoalsToWin")] public int worldCoopGoalsToWin;
    #endregion

    #region GoldenGoose
    [FormerlySerializedAs("GGGameLength")] [Header("Golden Goose Settings")]
    public float goldenGooseGameLength;
    [FormerlySerializedAs("GGSecondsPerPoint")] public float goldenGooseSecondsPerPoint;
    [FormerlySerializedAs("GGPickUpDelay")] public float goldenGoosePickUpDelay;
    [FormerlySerializedAs("GGMinBisonUptime")] public float goldenGooseMinBisonUptime;
    [FormerlySerializedAs("GGMaxBisonUptime")] public float goldenGooseMaxBisonUptime;
    #endregion

}
