using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EventSystem;

public class OperatorMenu : MonoBehaviour
{
    [Header("Navigation events")]

    [SerializeField]
    private GameEvent onDisableSettingsCategory;
    private List<GameObject> _minigameSettingsMenus = new List<GameObject>();
    private GameObject _currentNormalSettingsMenu;
    private GameObject _currentSubmenuSettingsMenu;

    #region Navigation

    public void OnEnableSettingsCategory(Component sender, object[] uiElementToEnableObj)
    {
        if (uiElementToEnableObj.First() is not GameObject uiElement) return;

        if (uiElement.GetComponent<OperatorNormalSettingPages>() != null)
        {
            onDisableSettingsCategory.Raise(this, _currentNormalSettingsMenu);
            _currentNormalSettingsMenu = uiElement;
        }

        if (uiElement.GetComponent<OperatorSubmenuPages>() != null)
        {
            onDisableSettingsCategory.Raise(this, _currentSubmenuSettingsMenu);
            _currentSubmenuSettingsMenu = uiElement;
        }

        _minigameSettingsMenus.Add(uiElement);
        uiElement.SetActive(true);
    }

    public void OnCloseAllSettingsFields()
    {
        if (_minigameSettingsMenus.Count <= 0) return;
        foreach (GameObject menu in _minigameSettingsMenus)
        {
            menu.SetActive(false);
        }
        _minigameSettingsMenus.Clear();
    }

    public void OnDisableSettingsCategory(Component sender, object[] activeElementObj)
    {
        if (activeElementObj.First() is not GameObject uiElement) return;
        uiElement.SetActive(false);
        _minigameSettingsMenus.Remove(uiElement);
    }
    #endregion

    #region Saving

    public void SaveSettings()
    {
        SettingsManager.Instance.SaveSettings();
    }

    public void RestoreDefaults()
    {
        SettingsManager.Instance.LoadDefault();
    }
    #endregion
}
