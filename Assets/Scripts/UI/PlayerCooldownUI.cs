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

            if (playercontroller.playerState == PlayerState.Dash)
            {
                SetAlpha(cooldownDashImage, 0.9f);
            }
            else
            {
                cooldownDashImage.fillAmount = playercontroller._dashCooldown / playercontroller.dashDelay;
                SetAlpha(cooldownDashImage, 0.85f);
            }

            if (playercontroller._dashCooldown <= 0 && playercontroller.playerState != PlayerState.Dash)
            {
                dashIsCoolingDown = false;
                cooldownDashImage.fillAmount = 0f;

                SetAlpha(cooldownDashImage, 0f);
            }
        }
        if (attackIsCoolingDown)
        {       

            if (playercontroller.playerState == PlayerState.Attack)
            {
                SetAlpha(cooldownAttackImage, 0.9f);
            }
            else
            {
                cooldownAttackImage.fillAmount = playercontroller._attackCooldown / playercontroller.attackDelay;
                SetAlpha(cooldownAttackImage, 0.85f);
            }

            if (playercontroller._attackCooldown <= 0 && playercontroller.playerState != PlayerState.Attack)
            {
                attackIsCoolingDown = false;
                cooldownAttackImage.fillAmount = 0f;

                SetAlpha(cooldownAttackImage, 0f);
            }
        }
        if (bracedIsCoolingDown)
        {       

            if (playercontroller.playerState == PlayerState.Braced)
            {
                SetAlpha(cooldownBracedImage, 0.9f);
            }
            else
            {
                cooldownBracedImage.fillAmount = playercontroller._bracedCooldown / playercontroller.bracedDelay;
                SetAlpha(cooldownBracedImage, 0.85f);
            }

            if (playercontroller._bracedCooldown <= 0 && playercontroller.playerState != PlayerState.Braced)
            {
                bracedIsCoolingDown = false;
                cooldownBracedImage.fillAmount = 0f;
                
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

            cooldownDashImage.fillAmount = 1f;

        }
    }
    public void StartAttackCooldown(Component sender, object[] _obj)
    {
        PlayerController pc = sender as PlayerController;
        if (!attackIsCoolingDown && pc.ID == id)
        {
            playercontroller = pc;
            attackIsCoolingDown = true;

            cooldownAttackImage.fillAmount = 1f;
        }
    }
    public void StartBracedCooldown(Component sender, object[] _obj)
    {
        PlayerController pc = sender as PlayerController;
        if (!bracedIsCoolingDown && pc.ID == id)
        {
            playercontroller = pc;
            bracedIsCoolingDown = true;

            cooldownBracedImage.fillAmount = 1f;        }
    }
    private void SetAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
