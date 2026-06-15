using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public SettingsData Settings;
    public static SettingsManager Instance;
    public string DefaultPath;

    private void Start()
    {
        //LOAD SETTINGS
        //LoadSettings(DefaultPath);
        Instance = this;
    }

    public void LoadSettings(string path)
    {
        Settings = Resources.Load<SettingsData>(path);
    }
}
