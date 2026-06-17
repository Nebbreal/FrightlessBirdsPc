using System.Collections.Generic;
using MinigameFramework;
using UnityEngine;
using UnityEngine.UI;
using Minigames;

public class OperatorMinigameChange : MonoBehaviour
{
    public Toggle FFToggle;
    public Toggle WCToggle;
    public Toggle GGToggle;

    public void OnEnable()
    {
        FFToggle.isOn = SettingsManager.Instance.Settings.FFActive;
        WCToggle.isOn = SettingsManager.Instance.Settings.WCActive;
        GGToggle.isOn = SettingsManager.Instance.Settings.GGActive;
    }

    public void ReloadMinigames()
    {
        var minigames = new List<Minigame>();
        if (SettingsManager.Instance.Settings.FFActive) minigames.Add(SettingsManager.Instance.FF);
        if (SettingsManager.Instance.Settings.WCActive) minigames.Add(SettingsManager.Instance.WC);
        if (SettingsManager.Instance.Settings.GGActive) minigames.Add(SettingsManager.Instance.GG);

        GameManager.Instance.Minigames = minigames;


        FFToggle.isOn = SettingsManager.Instance.Settings.FFActive;
        WCToggle.isOn = SettingsManager.Instance.Settings.WCActive;
        GGToggle.isOn = SettingsManager.Instance.Settings.GGActive;
    }

    public void OnChangeFF(bool value)
    {
        SettingsManager.Instance.Settings.FFActive = value;
        ReloadMinigames();
    }

    public void OnChangeWC(bool value)
    {
        SettingsManager.Instance.Settings.WCActive = value;
        ReloadMinigames();
    }

    public void OnChangeGG(bool value)
    {
        SettingsManager.Instance.Settings.GGActive = value;
        ReloadMinigames();
    }
}
