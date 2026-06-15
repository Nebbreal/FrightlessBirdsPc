using System.IO;
using System.Text;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public SettingsData Settings;
    public static SettingsManager Instance;
    public string DefaultPath;
    public string SavePath;

    public bool SaveToFile = false;

    private void Start()
    {
        //LOAD SETTINGS
        //LoadSettings(DefaultPath);
        Instance = this;
    }

    private void Update()
    {
        if (SaveToFile)
        {
            SaveToFile = false;
            SaveSettings();
        }
    }

    public void LoadSettings(string path)
    {
        var jsonTextFile = File.ReadAllText(path);
        Settings = JsonUtility.FromJson<SettingsData>(jsonTextFile);
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(Settings, true);
        File.WriteAllText(SavePath, json, new UTF8Encoding());
    }
}
