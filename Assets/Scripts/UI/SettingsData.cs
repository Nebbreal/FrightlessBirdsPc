using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Scriptable Objects/SettingsData")]
public class SettingsData : ScriptableObject
{
    // public List<ScriptableObject> settingsCategories = new List<ScriptableObject>();

    public GeneralSettings General;
    public MinigameSettings Minigames;
    public VolumeSettings Volume;
    public FrozenfeathersSettings FrozenFeathers;
    public WorldcoopSettings WorldCoop;
    public GoldengooseSettings GoldenGoose;

}
