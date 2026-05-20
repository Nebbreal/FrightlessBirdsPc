using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerCooldownUI : MonoBehaviour
{
    [Header("Cooldown Images")]
    public Image cooldownDashImage;
    public int id;
    //public Image cooldownAttackImage;
    //public Image cooldownBraceImage;

    private bool isCoolingDown;

    private PlayerController playercontroller;

    void Start()
    {

        cooldownDashImage.fillAmount = 1f;
    }

    void Update()
    {

        if (isCoolingDown)
        {        

            cooldownDashImage.fillAmount = 1 - (playercontroller._dashCooldown / playercontroller.dashDelay);

            if (playercontroller._dashCooldown <= 0 && playercontroller.playerState != PlayerState.Dash)
            {
                isCoolingDown = false;
                cooldownDashImage.fillAmount = 1f;
            }
        }
    }

    public void StartCooldown(Component sender, object[] _obj)
    {
        PlayerController pc = sender as PlayerController;
        if (!isCoolingDown && pc.ID == id)
        {
            playercontroller = pc;
            isCoolingDown = true;

            cooldownDashImage.fillAmount = 0f;
        }
    }
}
