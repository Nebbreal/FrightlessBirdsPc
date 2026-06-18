using UnityEngine;
using EventSystem;

public class OperatorConfirmClose : MonoBehaviour
{
    [SerializeField] private GameObject _generalSettingsElement;
    [SerializeField] private GameObject _confirmCloseElement;

    [SerializeField] private GameEvent _onEnableSettingsCategory;
    [SerializeField] private GameEvent _onDisableSettingsCategory;

    public void HandleOnClick()
    {
        _onEnableSettingsCategory.Raise((this), _generalSettingsElement);
        _onDisableSettingsCategory.Raise((this), _confirmCloseElement);
    }
}
