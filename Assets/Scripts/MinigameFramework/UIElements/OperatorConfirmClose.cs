using UnityEngine;
using EventSystem;
using UnityEngine.Serialization;

public class OperatorConfirmClose : MonoBehaviour
{
    [FormerlySerializedAs("_generalSettingsElement")] [SerializeField] private GameObject generalSettingsElement;
    [FormerlySerializedAs("_confirmCloseElement")] [SerializeField] private GameObject confirmCloseElement;

    [FormerlySerializedAs("_onEnableSettingsCategory")] [SerializeField] private GameEvent onEnableSettingsCategory;
    [FormerlySerializedAs("_onDisableSettingsCategory")] [SerializeField] private GameEvent onDisableSettingsCategory;

    public void HandleOnClick()
    {
        onEnableSettingsCategory.Raise((this), generalSettingsElement);
        onDisableSettingsCategory.Raise((this), confirmCloseElement);
    }
}
