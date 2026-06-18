using UnityEngine;
using EventSystem; 

public class OperatorNavigationButton : MonoBehaviour
{
    [SerializeField] private GameObject _uiElement;
    [SerializeField] private GameEvent _onEnableSettingsCategory;
    [SerializeField] private GameEvent _onCloseAllSettingsFields;

    

    public void HandleOnClick()
    {
        _onEnableSettingsCategory.Raise((this), _uiElement);
    }

    public void HandleCloseAllSettings()
    {
        _onCloseAllSettingsFields.Raise(this, null);
    }
}
