using EventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class TogglePlayerCooldownUI : MonoBehaviour
{

    public GameEvent EventPlayerUI;
    public GameEvent EventMenuUI;

    private int SwitchPlayerUI;

    private void Start()
    {
        SwitchPlayerUI = 0;
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
        if (SwitchPlayerUI == 0)
        {
            SwitchPlayerUI = 1;
            EventMenuUI.Raise(this);
        }
        else if (SwitchPlayerUI == 1)
        {
            SwitchPlayerUI = 0;        
            EventPlayerUI.Raise(this);
        }
    }
}
