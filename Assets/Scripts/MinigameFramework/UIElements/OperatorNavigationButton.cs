using UnityEngine;
using EventSystem;
using UnityEngine.Serialization;

public class OperatorNavigationButton : MonoBehaviour
{
    [FormerlySerializedAs("_uiElement")] [SerializeField] private GameObject uiElement;
    [FormerlySerializedAs("_onEnableSettingsCategory")] [SerializeField] private GameEvent onEnableSettingsCategory;
    [FormerlySerializedAs("_onCloseAllSettingsFields")] [SerializeField] private GameEvent onCloseAllSettingsFields;

    public void HandleOnClick()
    {
        onEnableSettingsCategory.Raise((this), uiElement);
    }

    public void HandleCloseAllSettings()
    {
        onCloseAllSettingsFields.Raise(this, null);
    }
}
