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
    public GameEvent OnSaveSettings;

    public OperatorGeneralChange OnGeneralChange;
    public OperatorMinigameChange OnMinigameChange;
    public OperatorVolumeChange OnVolumeChange;
    public OperatorFrozenfeathersChange OnFrozenfeathersChange;
    public OperatorWorldcoopChange OnWorldcoopChange;
    public OperatorGoldengooseChange OnGoldengooseChange;

    public bool SaveToFile = false;

    private void Start()
    {
        //LOAD SETTINGS
        if (!Instance) Instance = this;
        CopySettings();

        SavePath = Application.dataPath + "/StreamingAssets/Save.json";
        DefaultPath = Application.dataPath + "/StreamingAssets/Default.json";

        SaveDefaultSettings();
        if (File.Exists(SavePath))
        {
            LoadSettings(SavePath);
        }
        else
        {
            Debug.Log("Save file not found, loading defaults");
            LoadSettings(DefaultPath);
        }

        ReloadSettings();
    }

    public void CopySettings()
    {
        var settings = ScriptableObject.CreateInstance<SettingsData>();
        settings.AttackCooldown = Instance.Settings.AttackCooldown;
        settings.BlockCooldown = Instance.Settings.BlockCooldown;
        settings.DashCooldown = Instance.Settings.DashCooldown;
        settings.PlayerMovementSpeed = Instance.Settings.PlayerMovementSpeed;
        settings.FFActive = Instance.Settings.FFActive;
        settings.FFLives = Instance.Settings.FFLives;
        settings.FFPlatformEndSize = Instance.Settings.FFPlatformEndSize;
        settings.FFPlatformShrinkTime = Instance.Settings.FFPlatformShrinkTime;
        settings.WCGameLength = Instance.Settings.WCGameLength;
        settings.WCActive = Instance.Settings.WCActive;
        settings.WCGoalsToWin = Instance.Settings.WCGoalsToWin;
        settings.GGGameLength = Instance.Settings.GGGameLength;
        settings.GGMaxBisonUptime = Instance.Settings.GGMaxBisonUptime;
        settings.GGMinBisonUptime = Instance.Settings.GGPickUpDelay;
        settings.GGPickUpDelay = Instance.Settings.GGPickUpDelay;
        settings.GGSecondsPerPoint = Instance.Settings.GGSecondsPerPoint;
        settings.GGActive = Instance.Settings.GGActive;
        settings.MasterVolume = Instance.Settings.MasterVolume;
        settings.MusicVolume = Instance.Settings.MusicVolume;
        settings.SfxVolume = Instance.Settings.SfxVolume;

        Instance.Settings = settings;
    }
    
    public void ReloadSettings()
    {
        var minigames = new List<Minigame>();
        if (Instance.Settings.FFActive) minigames.Add(FF);
        if (Instance.Settings.WCActive) minigames.Add(WC);
        if (Instance.Settings.GGActive) minigames.Add(GG);

        GameManager.Instance.Minigames = minigames;

        OnMinigameChange.FFToggle.isOn = Instance.Settings.FFActive;
        OnMinigameChange.WCToggle.isOn = Instance.Settings.WCActive;
        OnMinigameChange.GGToggle.isOn = Instance.Settings.GGActive;

        //INITIALIZE VOLUME SLIDERS
        OnVolumeChange.MasterMixer.SetFloat("MasterVolumeParam", -40f*(1f-Instance.Settings.MasterVolume / 100f));
        OnVolumeChange.MasterMixer.SetFloat("MusicVolumeParam", -40f*(1f-Instance.Settings.MusicVolume / 100f));
        OnVolumeChange.MasterMixer.SetFloat("SFXVolumeParam", -40f*(1f-Instance.Settings.SfxVolume / 100f));
        OnVolumeChange.MasterVolumeSlider.value = Instance.Settings.MasterVolume;
        OnVolumeChange.MusicVolumeSlider.value = Instance.Settings.MusicVolume;
        OnVolumeChange.SFXVolumeSlider.value = Instance.Settings.SfxVolume;
        OnVolumeChange.MasterText.text = OnVolumeChange.MasterVolumeSlider.value.ToString("0");
        OnVolumeChange.MusicText.text = OnVolumeChange.MusicVolumeSlider.value.ToString("0");
        OnVolumeChange.SFXText.text = OnVolumeChange.SFXVolumeSlider.value.ToString("0");

        //INITIALIZE GENERAL SLIDERS
        OnGeneralChange.DashCooldownSlider.value = Instance.Settings.DashCooldown;
        OnGeneralChange.AttackCooldownSlider.value = Instance.Settings.AttackCooldown;
        OnGeneralChange.BlockDurationSlider.value = Instance.Settings.BlockCooldown;
        OnGeneralChange.PlayerAccelerationSlider.value = Instance.Settings.PlayerMovementSpeed;
        OnGeneralChange.DashCooldownText.text = OnGeneralChange.DashCooldownSlider.value.ToString("0" + "s");
        OnGeneralChange.AttackCooldownText.text = OnGeneralChange.AttackCooldownSlider.value.ToString("0" + "s");
        OnGeneralChange.BlockDurationText.text = OnGeneralChange.BlockDurationSlider.value.ToString("0" + "s");
        OnGeneralChange.PlayerAccelerationText.text = OnGeneralChange.PlayerAccelerationSlider.value.ToString("0");

        //INITIALIZE FROZEN FEATHERS SLIDERS
        OnFrozenfeathersChange.PlatformShrinkTargetSlider.value = Instance.Settings.FFPlatformEndSize;
        OnFrozenfeathersChange.LivesSlider.value = Instance.Settings.FFLives;
        OnFrozenfeathersChange.PlatformShrinkTimeSlider.value = Instance.Settings.FFPlatformShrinkTime;
        OnFrozenfeathersChange.PlatformShrinkTargetText.text = OnFrozenfeathersChange.PlatformShrinkTargetSlider.value.ToString("0");
        OnFrozenfeathersChange.LivesText.text = OnFrozenfeathersChange.LivesSlider.value.ToString("0");
        OnFrozenfeathersChange.PlatformShrinkTimeText.text = OnFrozenfeathersChange.PlatformShrinkTimeSlider.value.ToString("0" + "s");

        //INITIALIZE WORLD COOP SLIDERS
        OnWorldcoopChange.GameLengthSlider.value = Instance.Settings.WCGameLength;
        OnWorldcoopChange.GoalsToWinSlider.value = Instance.Settings.WCGoalsToWin;
        OnWorldcoopChange.GameLengthText.text = OnWorldcoopChange.GameLengthSlider.value.ToString("0" + "s");
        OnWorldcoopChange.GoalsToWinText.text = OnWorldcoopChange.GoalsToWinSlider.value.ToString("0");

        //INITIALIZE GOLDEN GOOSE SLIDERS
        OnGoldengooseChange.GameLengthSlider.value = Instance.Settings.GGGameLength;
        OnGoldengooseChange.SecondsPerPointSlider.value = Instance.Settings.GGSecondsPerPoint;
        OnGoldengooseChange.PickUpDelaySlider.value = Instance.Settings.GGPickUpDelay;
        OnGoldengooseChange.MinBisonUptimeSlider.value = Instance.Settings.GGMinBisonUptime;
        OnGoldengooseChange.MaxBisonUptimeSlider.value = Instance.Settings.GGMaxBisonUptime;
        OnGoldengooseChange.GameLengthText.text = OnGoldengooseChange.GameLengthSlider.value.ToString("0" + "s");
        OnGoldengooseChange.SecondsPerPointText.text = OnGoldengooseChange.SecondsPerPointSlider.value.ToString("0" + "s");
        OnGoldengooseChange.PickUpDelayText.text = OnGoldengooseChange.PickUpDelaySlider.value.ToString("0.0" + "s");
        OnGoldengooseChange.MinBisonUptimeText.text = OnGoldengooseChange.MinBisonUptimeSlider.value.ToString("0" + "s");
        OnGoldengooseChange.MaxBisonUptimeText.text = OnGoldengooseChange.MaxBisonUptimeSlider.value.ToString("0" + "s");
    }

    private void Update()
    {
        #if UNITY_EDITOR
        if (SaveToFile)
        {
            SaveToFile = false;
            SaveSettings();
        }
        #endif
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
        OnSaveSettings.Raise(this);
    }

    public void SaveDefaultSettings()
    {
        string json = JsonUtility.ToJson(Instance.Settings, true);
        File.WriteAllText(DefaultPath, json, new UTF8Encoding());
        OnSaveSettings.Raise(this);
    }
}
