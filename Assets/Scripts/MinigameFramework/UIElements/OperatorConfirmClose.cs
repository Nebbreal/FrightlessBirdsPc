using UnityEngine;
using EventSystem;

public class OperatorConfirmClose : MonoBehaviour
{
    [SerializeField] private GameObject generalSettingsElement;
    [SerializeField] private GameObject confirmCloseElement;

    [SerializeField] private GameEvent onEnableSettingsCategory;
    [SerializeField] private GameEvent onDisableSettingsCategory;

    public void HandleOnClick()
    {
        onEnableSettingsCategory.Raise((this), generalSettingsElement);
        onDisableSettingsCategory.Raise((this), confirmCloseElement);
    }
}
