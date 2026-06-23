using UnityEngine;
using EventSystem;
using UnityEngine.Serialization;

public class OperatorNavigationButton : MonoBehaviour
{
    [SerializeField] private GameObject uiElement;
    [SerializeField] private GameEvent onEnableSettingsCategory;
    [SerializeField] private GameEvent onCloseAllSettingsFields;

    public void HandleOnClick()
    {
        onEnableSettingsCategory.Raise((this), uiElement);
    }

    public void HandleCloseAllSettings()
    {
        onCloseAllSettingsFields.Raise(this, null);
    }
}
