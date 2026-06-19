using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using MinigameFramework;
using Minigames;
using EventSystem;
using UnityEngine.Serialization;

public class SettingsManager : MonoBehaviour
{
    [FormerlySerializedAs("Settings")] public SettingsData settings;
    public static SettingsManager Instance;
    [FormerlySerializedAs("DefaultPath")] public string defaultPath;
    [FormerlySerializedAs("SavePath")] public string savePath;

    [FormerlySerializedAs("FF")] public Minigame frozenFeathers;
    [FormerlySerializedAs("WC")] public Minigame worldCoop;
    [FormerlySerializedAs("GG")] public Minigame goldenGoose;

    [FormerlySerializedAs("RestoreDefaults")] public GameEvent restoreDefaults;
    [FormerlySerializedAs("OnSaveSettings")] public GameEvent onSaveSettings;

    [FormerlySerializedAs("OnGeneralChange")] public OperatorGeneralChange onGeneralChange;
    [FormerlySerializedAs("OnMinigameChange")] public OperatorMinigameChange onMinigameChange;
    [FormerlySerializedAs("OnVolumeChange")] public OperatorVolumeChange onVolumeChange;
    [FormerlySerializedAs("OnFrozenfeathersChange")] public OperatorFrozenfeathersChange onFrozenfeathersChange;
    [FormerlySerializedAs("OnWorldcoopChange")] public OperatorWorldcoopChange onWorldcoopChange;
    [FormerlySerializedAs("OnGoldengooseChange")] public OperatorGoldengooseChange onGoldengooseChange;

    [FormerlySerializedAs("SaveToFile")] public bool saveToFile = false;

    private void Start()
    {
        //LOAD SETTINGS
        if (!Instance) Instance = this;
        CopySettings();

        savePath = Application.dataPath + "/StreamingAssets/Save.json";
        defaultPath = Application.dataPath + "/StreamingAssets/Default.json";

        SaveDefaultSettings();
        if (File.Exists(savePath))
        {
            LoadSettings(savePath);
        }
        else
        {
            Debug.Log("Save file not found, loading defaults");
            LoadSettings(defaultPath);
        }

        ReloadSettings();
    }

    public void CopySettings()
    {
        var settings = ScriptableObject.CreateInstance<SettingsData>();
        settings.attackCooldown = Instance.settings.attackCooldown;
        settings.blockCooldown = Instance.settings.blockCooldown;
        settings.dashCooldown = Instance.settings.dashCooldown;
        settings.playerMovementSpeed = Instance.settings.playerMovementSpeed;
        settings.frozenFeathersActive = Instance.settings.frozenFeathersActive;
        settings.frozenFeathersLives = Instance.settings.frozenFeathersLives;
        settings.frozenFeathersPlatformEndSize = Instance.settings.frozenFeathersPlatformEndSize;
        settings.frozenFeathersPlatformShrinkTime = Instance.settings.frozenFeathersPlatformShrinkTime;
        settings.worldCoopGameLength = Instance.settings.worldCoopGameLength;
        settings.worldCoopActive = Instance.settings.worldCoopActive;
        settings.worldCoopGoalsToWin = Instance.settings.worldCoopGoalsToWin;
        settings.goldenGooseGameLength = Instance.settings.goldenGooseGameLength;
        settings.goldenGooseMaxBisonUptime = Instance.settings.goldenGooseMaxBisonUptime;
        settings.goldenGooseMinBisonUptime = Instance.settings.goldenGoosePickUpDelay;
        settings.goldenGoosePickUpDelay = Instance.settings.goldenGoosePickUpDelay;
        settings.goldenGooseSecondsPerPoint = Instance.settings.goldenGooseSecondsPerPoint;
        settings.goldenGooseActive = Instance.settings.goldenGooseActive;
        settings.masterVolume = Instance.settings.masterVolume;
        settings.musicVolume = Instance.settings.musicVolume;
        settings.sfxVolume = Instance.settings.sfxVolume;

        Instance.settings = settings;
    }
    
    public void ReloadSettings()
    {
        var minigames = new List<Minigame>();
        if (Instance.settings.frozenFeathersActive) minigames.Add(frozenFeathers);
        if (Instance.settings.worldCoopActive) minigames.Add(worldCoop);
        if (Instance.settings.goldenGooseActive) minigames.Add(goldenGoose);

        GameManager.Instance.minigames = minigames;

        onMinigameChange.frozenFeathersToggle.isOn = Instance.settings.frozenFeathersActive;
        onMinigameChange.worldCoopToggle.isOn = Instance.settings.worldCoopActive;
        onMinigameChange.goldenGooseToggle.isOn = Instance.settings.goldenGooseActive;

        //INITIALIZE VOLUME SLIDERS
        onVolumeChange.masterMixer.SetFloat("MasterVolumeParam", -40f*(1f-Instance.settings.masterVolume / 100f));
        onVolumeChange.masterMixer.SetFloat("MusicVolumeParam", -40f*(1f-Instance.settings.musicVolume / 100f));
        onVolumeChange.masterMixer.SetFloat("SFXVolumeParam", -40f*(1f-Instance.settings.sfxVolume / 100f));
        onVolumeChange.masterVolumeSlider.value = Instance.settings.masterVolume;
        onVolumeChange.musicVolumeSlider.value = Instance.settings.musicVolume;
        onVolumeChange.sfxVolumeSlider.value = Instance.settings.sfxVolume;
        onVolumeChange.masterText.text = onVolumeChange.masterVolumeSlider.value.ToString("0");
        onVolumeChange.musicText.text = onVolumeChange.musicVolumeSlider.value.ToString("0");
        onVolumeChange.sfxText.text = onVolumeChange.sfxVolumeSlider.value.ToString("0");

        //INITIALIZE GENERAL SLIDERS
        onGeneralChange.dashCooldownSlider.value = Instance.settings.dashCooldown;
        onGeneralChange.attackCooldownSlider.value = Instance.settings.attackCooldown;
        onGeneralChange.blockDurationSlider.value = Instance.settings.blockCooldown;
        onGeneralChange.playerAccelerationSlider.value = Instance.settings.playerMovementSpeed;
        onGeneralChange.dashCooldownText.text = onGeneralChange.dashCooldownSlider.value.ToString("0" + "s");
        onGeneralChange.attackCooldownText.text = onGeneralChange.attackCooldownSlider.value.ToString("0" + "s");
        onGeneralChange.blockDurationText.text = onGeneralChange.blockDurationSlider.value.ToString("0" + "s");
        onGeneralChange.playerAccelerationText.text = onGeneralChange.playerAccelerationSlider.value.ToString("0");

        //INITIALIZE FROZEN FEATHERS SLIDERS
        onFrozenfeathersChange.platformShrinkTargetSlider.value = Instance.settings.frozenFeathersPlatformEndSize;
        onFrozenfeathersChange.livesSlider.value = Instance.settings.frozenFeathersLives;
        onFrozenfeathersChange.platformShrinkTimeSlider.value = Instance.settings.frozenFeathersPlatformShrinkTime;
        onFrozenfeathersChange.platformShrinkTargetText.text = onFrozenfeathersChange.platformShrinkTargetSlider.value.ToString("0");
        onFrozenfeathersChange.livesText.text = onFrozenfeathersChange.livesSlider.value.ToString("0");
        onFrozenfeathersChange.platformShrinkTimeText.text = onFrozenfeathersChange.platformShrinkTimeSlider.value.ToString("0" + "s");

        //INITIALIZE WORLD COOP SLIDERS
        onWorldcoopChange.gameLengthSlider.value = Instance.settings.worldCoopGameLength;
        onWorldcoopChange.goalsToWinSlider.value = Instance.settings.worldCoopGoalsToWin;
        onWorldcoopChange.gameLengthText.text = onWorldcoopChange.gameLengthSlider.value.ToString("0" + "s");
        onWorldcoopChange.goalsToWinText.text = onWorldcoopChange.goalsToWinSlider.value.ToString("0");

        //INITIALIZE GOLDEN GOOSE SLIDERS
        onGoldengooseChange.gameLengthSlider.value = Instance.settings.goldenGooseGameLength;
        onGoldengooseChange.secondsPerPointSlider.value = Instance.settings.goldenGooseSecondsPerPoint;
        onGoldengooseChange.pickUpDelaySlider.value = Instance.settings.goldenGoosePickUpDelay;
        onGoldengooseChange.minBisonUptimeSlider.value = Instance.settings.goldenGooseMinBisonUptime;
        onGoldengooseChange.maxBisonUptimeSlider.value = Instance.settings.goldenGooseMaxBisonUptime;
        onGoldengooseChange.gameLengthText.text = onGoldengooseChange.gameLengthSlider.value.ToString("0" + "s");
        onGoldengooseChange.secondsPerPointText.text = onGoldengooseChange.secondsPerPointSlider.value.ToString("0" + "s");
        onGoldengooseChange.pickUpDelayText.text = onGoldengooseChange.pickUpDelaySlider.value.ToString("0.0" + "s");
        onGoldengooseChange.minBisonUptimeText.text = onGoldengooseChange.minBisonUptimeSlider.value.ToString("0" + "s");
        onGoldengooseChange.maxBisonUptimeText.text = onGoldengooseChange.maxBisonUptimeSlider.value.ToString("0" + "s");
    }

    private void Update()
    {
        #if UNITY_EDITOR
        if (saveToFile)
        {
            saveToFile = false;
            SaveSettings();
        }
        #endif
    }

    public void LoadDefault(){
        
        LoadSettings(defaultPath);
        restoreDefaults.Raise(this);
    }

    public void LoadSettings(string path)
    {
        var jsonTextFile = File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(jsonTextFile, Instance.settings);
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(Instance.settings, true);
        File.WriteAllText(savePath, json, new UTF8Encoding());
        onSaveSettings.Raise(this);
    }

    public void SaveDefaultSettings()
    {
        string json = JsonUtility.ToJson(Instance.settings, true);
        File.WriteAllText(defaultPath, json, new UTF8Encoding());
        onSaveSettings.Raise(this);
    }
}
