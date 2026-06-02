using MinigameFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCooldownUI : MonoBehaviour
{
    [Header("Cooldown Images")]
    public Image cooldownDashImage;
    public Image cooldownAttackImage;
    public Image cooldownBracedImage;

    [Header("Player ID")]
    public int id;
    private bool dashIsCoolingDown;
    private bool attackIsCoolingDown;
    private bool bracedIsCoolingDown;

    [Header("Cooldown Images Color")]
    public Image cooldownDashBackgroundColor;
    public Image cooldownAttackBackgroundColor;
    public Image cooldownBracedBackgroundColor;

    private PlayerController playercontroller;

    private void Start()
    {

        cooldownDashImage.fillAmount = 1f;
        cooldownAttackImage.fillAmount = 1f;
        cooldownBracedImage.fillAmount = 1f;

        SetCooldownColor();
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
    private void SetCooldownColor()
    {
        PlayerManager playerManager = FindFirstObjectByType<PlayerManager>();

            Color playerColor = playerManager.PlayerColors[id];

            cooldownDashBackgroundColor.color = playerColor;
            cooldownAttackBackgroundColor.color = playerColor;
            cooldownBracedBackgroundColor.color = playerColor;
    }

    private void Update()
    {

        if (dashIsCoolingDown)
        {       
            cooldownDashImage.fillAmount = 1 - (playercontroller._dashCooldown / playercontroller.dashDelay);

            if (playercontroller.playerState == PlayerState.Dash)
            {
                SetAlpha(cooldownDashImage, 0.6f);
            }
            else
            {
                SetAlpha(cooldownDashImage, 0.4f);
            }

            if (playercontroller._dashCooldown <= 0 && playercontroller.playerState != PlayerState.Dash)
            {
                dashIsCoolingDown = false;
                cooldownDashImage.fillAmount = 1f;

                SetAlpha(cooldownDashImage, 0f);
            }
        }
        if (attackIsCoolingDown)
        {       
            cooldownAttackImage.fillAmount = 1 - (playercontroller._attackCooldown / playercontroller.attackDelay);

            if (playercontroller.playerState == PlayerState.Attack)
            {
                SetAlpha(cooldownAttackImage, 0.6f);
            }
            else
            {
                SetAlpha(cooldownAttackImage, 0.4f);
            }

            if (playercontroller._attackCooldown <= 0 && playercontroller.playerState != PlayerState.Attack)
            {
                attackIsCoolingDown = false;
                cooldownAttackImage.fillAmount = 1f;

                SetAlpha(cooldownAttackImage, 0f);
            }
        }
        if (bracedIsCoolingDown)
        {       
            cooldownBracedImage.fillAmount = 1 - (playercontroller._bracedCooldown / playercontroller.bracedDelay);

            if (playercontroller.playerState == PlayerState.Braced)
            {
                SetAlpha(cooldownBracedImage, 0.6f);
            }
            else
            {
                SetAlpha(cooldownBracedImage, 0.4f);
            }

            if (playercontroller._bracedCooldown <= 0 && playercontroller.playerState != PlayerState.Braced)
            {
                bracedIsCoolingDown = false;
                cooldownBracedImage.fillAmount = 1f;
                
                SetAlpha(cooldownBracedImage, 0f);
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
        }
    }
    public void StartBracedCooldown(Component sender, object[] _obj)
    {
        PlayerController pc = sender as PlayerController;
        if (!bracedIsCoolingDown && pc.ID == id)
        {
            playercontroller = pc;
            bracedIsCoolingDown = true;

            cooldownBracedImage.fillAmount = 0f;        }
    }
    private void SetAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
