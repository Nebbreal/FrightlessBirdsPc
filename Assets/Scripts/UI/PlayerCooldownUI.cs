using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerCooldownUI : MonoBehaviour
{
    [Header("Cooldown Images")]
    public Image cooldownDashImage;
    public Image cooldownAttackImage;
    public Image cooldownBracedImage;

    public int id;
    private bool dashIsCoolingDown;
    private bool attackIsCoolingDown;
    private bool bracedIsCoolingDown;

    private PlayerController playercontroller;

    private void Start()
    {

        cooldownDashImage.fillAmount = 1f;
        cooldownAttackImage.fillAmount = 1f;
        cooldownBracedImage.fillAmount = 1f;
    }

    private void OnEnable()
    {
        var playerController = GetComponentInParent<PlayerController>();
        if (playerController)
        {
            id = playerController.ID;
            playercontroller = playerController;
        }
    }

    private void Update()
    {

        if (dashIsCoolingDown)
        {       
            cooldownDashImage.fillAmount = 1 - (playercontroller._dashCooldown / playercontroller.dashDelay);

            if (playercontroller._dashCooldown <= 0 && playercontroller.playerState != PlayerState.Dash)
            {
                dashIsCoolingDown = false;
                cooldownDashImage.fillAmount = 1f;
                cooldownDashImage.color = Color.white;
            }
        }
        if (attackIsCoolingDown)
        {       
            cooldownAttackImage.fillAmount = 1 - (playercontroller._attackCooldown / playercontroller.attackDelay);

            if (playercontroller._attackCooldown <= 0 && playercontroller.playerState != PlayerState.Attack)
            {
                attackIsCoolingDown = false;
                cooldownAttackImage.fillAmount = 1f;
                cooldownAttackImage.color = Color.white;
            }
        }
        if (bracedIsCoolingDown)
        {       
            cooldownBracedImage.fillAmount = 1 - (playercontroller._bracedCooldown / playercontroller.bracedDelay);

            if (playercontroller._bracedCooldown <= 0 && playercontroller.playerState != PlayerState.Braced)
            {
                bracedIsCoolingDown = false;
                cooldownBracedImage.fillAmount = 1f;
                cooldownBracedImage.color = Color.white;
            }
        }

    }

    public void StartDashCooldown(Component sender, object[] _obj)
    {
        PlayerController pc = sender as PlayerController;
        if (!dashIsCoolingDown && pc.ID == id)
        {
            playercontroller = pc;
            dashIsCoolingDown = true;

            cooldownDashImage.fillAmount = 0f;
            cooldownDashImage.color = Color.gray8;
        }
    }
    public void StartAttackCooldown(Component sender, object[] _obj)
    {
        PlayerController pc = sender as PlayerController;
        if (!attackIsCoolingDown && pc.ID == id)
        {
            playercontroller = pc;
            attackIsCoolingDown = true;

            cooldownAttackImage.fillAmount = 0f;
            cooldownAttackImage.color = Color.gray8;
        }
    }
    public void StartBracedCooldown(Component sender, object[] _obj)
    {
        PlayerController pc = sender as PlayerController;
        if (!bracedIsCoolingDown && pc.ID == id)
        {
            playercontroller = pc;
            bracedIsCoolingDown = true;

            cooldownBracedImage.fillAmount = 0f;
            cooldownBracedImage.color = Color.gray8;
        }
    }
}
