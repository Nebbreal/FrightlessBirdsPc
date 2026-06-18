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
        LoadSettings(SavePath);
        ReloadSettings();
    }
    public void ReloadSettings()
    {
        var minigames = new List<Minigame>();
        if (Instance.Settings.FFActive) minigames.Add(FF);
        if (Instance.Settings.WCActive) minigames.Add(WC);
        if (Instance.Settings.GGActive) minigames.Add(GG);

        GameManager.Instance.Minigames = minigames;

        OnMinigameChange.FFToggle.isOn = SettingsManager.Instance.Settings.FFActive;
        OnMinigameChange.WCToggle.isOn = SettingsManager.Instance.Settings.WCActive;
        OnMinigameChange.GGToggle.isOn = SettingsManager.Instance.Settings.GGActive;

        //INITIALIZE VOLUME SLIDERS
        OnVolumeChange.MasterMixer.SetFloat("MasterVolumeParam", -40f*(1f-SettingsManager.Instance.Settings.MasterVolume / 100f));
        OnVolumeChange.MasterMixer.SetFloat("MusicVolumeParam", -40f*(1f-SettingsManager.Instance.Settings.MusicVolume / 100f));
        OnVolumeChange.MasterMixer.SetFloat("SFXVolumeParam", -40f*(1f-SettingsManager.Instance.Settings.SfxVolume / 100f));
        OnVolumeChange.MasterVolumeSlider.value = SettingsManager.Instance.Settings.MasterVolume;
        OnVolumeChange.MusicVolumeSlider.value = SettingsManager.Instance.Settings.MusicVolume;
        OnVolumeChange.SFXVolumeSlider.value = SettingsManager.Instance.Settings.SfxVolume;
        OnVolumeChange.MasterText.text = OnVolumeChange.MasterVolumeSlider.value.ToString("0");
        OnVolumeChange.MusicText.text = OnVolumeChange.MusicVolumeSlider.value.ToString("0");
        OnVolumeChange.SFXText.text = OnVolumeChange.SFXVolumeSlider.value.ToString("0");

        //INITIALIZE GENERAL SLIDERS
        OnGeneralChange.DashCooldownSlider.value = SettingsManager.Instance.Settings.DashCooldown;
        OnGeneralChange.AttackCooldownSlider.value = SettingsManager.Instance.Settings.AttackCooldown;
        OnGeneralChange.BlockDurationSlider.value = SettingsManager.Instance.Settings.BlockCooldown;
        OnGeneralChange.PlayerAccelerationSlider.value = SettingsManager.Instance.Settings.PlayerMovementSpeed;
        OnGeneralChange.DashCooldownText.text = OnGeneralChange.DashCooldownSlider.value.ToString("0" + "s");
        OnGeneralChange.AttackCooldownText.text = OnGeneralChange.AttackCooldownSlider.value.ToString("0" + "s");
        OnGeneralChange.BlockDurationText.text = OnGeneralChange.BlockDurationSlider.value.ToString("0" + "s");
        OnGeneralChange.PlayerAccelerationText.text = OnGeneralChange.PlayerAccelerationSlider.value.ToString("0");

        //INITIALIZE FROZEN FEATHERS SLIDERS
        OnFrozenfeathersChange.PlatformShrinkTargetSlider.value = SettingsManager.Instance.Settings.FFPlatformEndSize;
        OnFrozenfeathersChange.LivesSlider.value = SettingsManager.Instance.Settings.FFLives;
        OnFrozenfeathersChange.PlatformShrinkTimeSlider.value = SettingsManager.Instance.Settings.FFPlatformShrinkTime;
        OnFrozenfeathersChange.PlatformShrinkTargetText.text = OnFrozenfeathersChange.PlatformShrinkTargetSlider.value.ToString("0");
        OnFrozenfeathersChange.LivesText.text = OnFrozenfeathersChange.LivesSlider.value.ToString("0");
        OnFrozenfeathersChange.PlatformShrinkTimeText.text = OnFrozenfeathersChange.PlatformShrinkTimeSlider.value.ToString("0" + "s");

        //INITIALIZE WORLD COOP SLIDERS
        OnWorldcoopChange.GameLengthSlider.value = SettingsManager.Instance.Settings.WCGameLength;
        OnWorldcoopChange.GoalsToWinSlider.value = SettingsManager.Instance.Settings.WCGoalsToWin;
        OnWorldcoopChange.GameLengthText.text = OnWorldcoopChange.GameLengthSlider.value.ToString("0" + "s");
        OnWorldcoopChange.GoalsToWinText.text = OnWorldcoopChange.GoalsToWinSlider.value.ToString("0");

        //INITIALIZE GOLDEN GOOSE SLIDERS
        OnGoldengooseChange.GameLengthSlider.value = SettingsManager.Instance.Settings.GGGameLength;
        OnGoldengooseChange.SecondsPerPointSlider.value = SettingsManager.Instance.Settings.GGSecondsPerPoint;
        OnGoldengooseChange.PickUpDelaySlider.value = SettingsManager.Instance.Settings.GGPickUpDelay;
        OnGoldengooseChange.MinBisonUptimeSlider.value = SettingsManager.Instance.Settings.GGMinBisonUptime;
        OnGoldengooseChange.MaxBisonUptimeSlider.value = SettingsManager.Instance.Settings.GGMaxBisonUptime;
        OnGoldengooseChange.GameLengthText.text = OnGoldengooseChange.GameLengthSlider.value.ToString("0" + "s");
        OnGoldengooseChange.SecondsPerPointText.text = OnGoldengooseChange.SecondsPerPointSlider.value.ToString("0.0" + "s");
        OnGoldengooseChange.PickUpDelayText.text = OnGoldengooseChange.PickUpDelaySlider.value.ToString("0.0" + "s");
        OnGoldengooseChange.MinBisonUptimeText.text = OnGoldengooseChange.MinBisonUptimeSlider.value.ToString("0" + "s");
        OnGoldengooseChange.MaxBisonUptimeText.text = OnGoldengooseChange.MaxBisonUptimeSlider.value.ToString("0" + "s");
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
        OnSaveSettings.Raise(this);
    }
}
