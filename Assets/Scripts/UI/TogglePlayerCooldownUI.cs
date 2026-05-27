using EventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class TogglePlayerCooldownUI : MonoBehaviour
{

    public GameEvent EventPlayerUI;
    public GameEvent EventMenuUI;

    private int SwitchCooldownUI;

    private void Start()
    {
        SwitchCooldownUI = 0;
    }
    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleUI();
        }

    }
    public void ToggleUI()
    {
        if (SwitchCooldownUI == 0)
        {
            SwitchCooldownUI = 1;
            EventPlayerUI.Raise(this);
        }
        else if (SwitchCooldownUI == 1)
        {
            SwitchCooldownUI = 0;        
            EventMenuUI.Raise(this);
        }
    }
}
