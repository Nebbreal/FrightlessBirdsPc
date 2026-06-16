using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using MinigameFramework;
using Minigames;
using EventSystem;

public class SettingsManager : MonoBehaviour
{
    public SettingsData Settings;
    public static SettingsManager Instance;
    public string DefaultPath;
    public string SavePath;

    public Minigame FF;
    public Minigame WC;
    public Minigame GG;

    public GameEvent RestoreDefaults;

    public OperatorVolumeChange OnVolumeChange;

    public bool SaveToFile = false;

    private void Start()
    {
        //LOAD SETTINGS
        if(!Instance) Instance = this;
        LoadSettings(SavePath);
        
        var minigames = new List<Minigame>();
        if (Instance.Settings.FFActive) minigames.Add(FF);
        if (Instance.Settings.WCActive) minigames.Add(WC);
        if (Instance.Settings.GGActive) minigames.Add(GG);

        GameManager.Instance.Minigames = minigames;

        //INITIALIZE VOLUME SLIDERS
        OnVolumeChange.MasterMixer.SetFloat("MasterVolumeParam", SettingsManager.Instance.Settings.MasterVolume);
        OnVolumeChange.MasterMixer.SetFloat("MusicVolumeParam", SettingsManager.Instance.Settings.MusicVolume);
        OnVolumeChange.MasterMixer.SetFloat("SFXVolumeParam", SettingsManager.Instance.Settings.SfxVolume);
        OnVolumeChange.MasterVolumeSlider.value = SettingsManager.Instance.Settings.MasterVolume;
        OnVolumeChange.MusicVolumeSlider.value = SettingsManager.Instance.Settings.MusicVolume;
        OnVolumeChange.SFXVolumeSlider.value = SettingsManager.Instance.Settings.SfxVolume;
        OnVolumeChange.MasterText.text = OnVolumeChange.MasterVolumeSlider.value.ToString("0");
        OnVolumeChange.MusicText.text = OnVolumeChange.MusicVolumeSlider.value.ToString("0");
        OnVolumeChange.SFXText.text = OnVolumeChange.SFXVolumeSlider.value.ToString("0");
    }

    private void Update()
    {
        if (SaveToFile)
        {
            SaveToFile = false;
            SaveSettings();
        }
    }

    public void LoadDefault(){
        
        LoadSettings(DefaultPath);
        RestoreDefaults.Raise(this);
    }

    public void LoadSettings(string path)
    {
        var jsonTextFile = File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(jsonTextFile, Instance.Settings);
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(Instance.Settings, true);
        File.WriteAllText(SavePath, json, new UTF8Encoding());
    }
}
