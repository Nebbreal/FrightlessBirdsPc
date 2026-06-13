using UnityEngine;
using EventSystem;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;

public class OperatorMenu : MonoBehaviour
{
    [Header("Navigation events")]
    public GameEvent onEnableOperatorMenuStart;
    public GameEvent onDisableOperatorMenuStart;
    public GameEvent onEnableGeneralSettings;
    public GameEvent onDisableGeneralSettings;
    public GameEvent onEnableMinigameSettings;
    public GameEvent onDisableMinigameSettings;
    public GameEvent onEnableVolumeSettings;
    public GameEvent onDisableVolumeSettings;
    public GameEvent onEnableApplyConfirm;
    public GameEvent onDisableApplyConfirm;
    public GameEvent onEnableExitConfirm;
    public GameEvent onDisableExitConfirm;
    public GameEvent onEnableRestoreDefaultsConfirm;
    public GameEvent onDisableRestoreDefaultsConfirm;

    [Header("Setting variables")]
    [SerializeField]
    public Slider DashCooldown;
    public TextMeshProUGUI DashCooldownText;

    #region Navigation
    public void OpenGeneralSettings()
    {
        CleanupActiveMenus();
        onEnableGeneralSettings.Raise((this));
    }
    public void OpenMinigameSettings()
    {
        CleanupActiveMenus();
        onEnableMinigameSettings.Raise((this));
    }
    public void OpenVolumeSettings()
    {
        CleanupActiveMenus();
        onEnableVolumeSettings.Raise((this));
    }
    
    public void CloseMinigameSettings()
    {
        onDisableMinigameSettings.Raise((this));
    }
    public void CloseVolumeSettings()
    {
        onDisableVolumeSettings.Raise((this));
    }
    public void CloseGeneralSettings()
    {
        onDisableGeneralSettings.Raise((this));
    }
    public void CloseOperatorMenuStartSettings()
    {
        onDisableOperatorMenuStart.Raise((this));
    }

    public void CleanupActiveMenus()
    {
        CloseGeneralSettings();
        CloseMinigameSettings();
        CloseVolumeSettings();
        CloseOperatorMenuStartSettings();
        CloseExitConfirm();
    }

    public void OpenApplyConfirm()
    {
        onEnableApplyConfirm.Raise(this);
    }
    public void OpenExitConfirm()
    {
        onEnableExitConfirm.Raise(this);
    }
    public void OpenRestoreDefaultsConfirm()
    {
        onEnableRestoreDefaultsConfirm.Raise(this);
    }

    public void CloseApplyConfirm()
    {
        onDisableApplyConfirm.Raise(this);
    }
    public void CloseExitConfirm()
    {
        onDisableExitConfirm.Raise(this);
    }
    public void CloseRestoreDefaultsConfirm()
    {
        onDisableRestoreDefaultsConfirm.Raise(this);
    }
    #endregion

    #region Saving

    public void ApplySettings()
    {
        Debug.Log("Settings Applied");
        onDisableApplyConfirm.Raise(this);
    }

    public void RestoreDefaults()
    {
        Debug.Log("Defaults Restored");
        onDisableRestoreDefaultsConfirm.Raise(this);
    }
    #endregion

    #region Changing settings

    public void Start()
    {
        //Slider value assigning
        DashCooldown.minValue = 5f;
        DashCooldown.maxValue = 10f;
        DashCooldown.value = 7.5f;
    }

    public void Update()
    {
        DashCooldownText.text = DashCooldown.value.ToString("0.0");
    }
    #endregion
}
