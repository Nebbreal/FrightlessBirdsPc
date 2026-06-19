using System.Collections.Generic;
using MinigameFramework;
using UnityEngine;
using UnityEngine.UI;
using Minigames;
using UnityEngine.Serialization;

public class OperatorMinigameChange : MonoBehaviour
{
    [FormerlySerializedAs("ffToggle")] [FormerlySerializedAs("FFToggle")] public Toggle frozenFeathersToggle;
    [FormerlySerializedAs("WCToggle")] public Toggle worldCoopToggle;
    [FormerlySerializedAs("GGToggle")] public Toggle goldenGooseToggle;

    public void OnEnable()
    {
        frozenFeathersToggle.isOn = SettingsManager.Instance.settings.frozenFeathersActive;
        worldCoopToggle.isOn = SettingsManager.Instance.settings.worldCoopActive;
        goldenGooseToggle.isOn = SettingsManager.Instance.settings.goldenGooseActive;
    }

    public void ReloadMinigames()
    {
        var minigames = new List<Minigame>();
        if (SettingsManager.Instance.settings.frozenFeathersActive) minigames.Add(SettingsManager.Instance.frozenFeathers);
        if (SettingsManager.Instance.settings.worldCoopActive) minigames.Add(SettingsManager.Instance.worldCoop);
        if (SettingsManager.Instance.settings.goldenGooseActive) minigames.Add(SettingsManager.Instance.goldenGoose);

        GameManager.Instance.minigames = minigames;
        
        frozenFeathersToggle.isOn = SettingsManager.Instance.settings.frozenFeathersActive;
        worldCoopToggle.isOn = SettingsManager.Instance.settings.worldCoopActive;
        goldenGooseToggle.isOn = SettingsManager.Instance.settings.goldenGooseActive;
    }

    public void OnChangeFF(bool value)
    {
        SettingsManager.Instance.settings.frozenFeathersActive = value;
        ReloadMinigames();
    }

    public void OnChangeWC(bool value)
    {
        SettingsManager.Instance.settings.worldCoopActive = value;
        ReloadMinigames();
    }

    public void OnChangeGG(bool value)
    {
        SettingsManager.Instance.settings.goldenGooseActive = value;
        ReloadMinigames();
    }
}
