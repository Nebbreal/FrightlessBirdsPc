using System.IO;
using System.Text;
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
        var jsonTextFile = File.ReadAllText(DefaultPath);
        Settings = JsonUtility.FromJson<SettingsData>(jsonTextFile);
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(Settings, true);
        File.WriteAllText(DefaultPath, json, new UTF8Encoding());
    }
}
