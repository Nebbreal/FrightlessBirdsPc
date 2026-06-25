using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Scriptable Objects/SettingsData")]
public class SettingsData : ScriptableObject
{
    #region General
    [Header("Player Settings")]
    public float dashCooldown;
    public float attackCooldown;
    public float blockCooldown;
    public float playerMovementSpeed;
    #endregion

    #region Minigames
    [Header("Active Minigame Settings")]
    public bool frozenFeathersActive;
    public bool worldCoopActive;
    public bool goldenGooseActive;
    #endregion

    #region Volume
    [Header("Volume Settings")]
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    #endregion

    #region FrozenFeathers [Header("Frozen Feathers Settings")]
    public float frozenFeathersPlatformEndSize;
    public float frozenFeathersLives;
    public float frozenFeathersPlatformShrinkTime;
    #endregion

    #region WorldCoop
    [Header("World Coop Settings")]
    public float worldCoopGameLength;
    public int worldCoopGoalsToWin;
    #endregion

    #region GoldenGoose
    [Header("Golden Goose Settings")]
    public float goldenGooseGameLength;
    public float goldenGooseSecondsPerPoint;
    public float goldenGoosePickUpDelay;
    public float goldenGooseMinBisonUptime;
    public float goldenGooseMaxBisonUptime;
    #endregion

}
