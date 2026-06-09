using MinigameFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCooldownUI : MonoBehaviour
{
    [Header("Cooldown Images Overlay")]
    public Image CooldownDashImage;
    public Image CooldownAttackImage;
    public Image CooldownBlockingImage;
    
    [Header("Cooldown Images Overlay Transparency")]
    public float inAction = 0.9f;
    public float coolingDown = 0.85f;
    public float canUseCooldown = 0f;

    [Header("Player ID")]
    public int id;
    private bool _dashIsCoolingDown;
    private bool _attackIsCoolingDown;
    private bool _BlockingIsCoolingDown;

    [Header("Cooldown Background Images")]
    public Image cooldownDashBackgroundImage;
    public Image cooldownAttackBackgroundImage;
    public Image cooldownBlockingBackgroundImage;
    public float cooldownBackgroundAlpha = 0.8f;
    public float cooldownBlockInUseColor = 0.4f;

    private PlayerController _playerController;

    private void Start()
    {
        CooldownDashImage.fillAmount = 1f;
        CooldownAttackImage.fillAmount = 1f;
        CooldownBlockingImage.fillAmount = 1f;

        SetCooldownColor();

        SetAlpha(cooldownDashBackgroundImage, cooldownBackgroundAlpha);
        SetAlpha(cooldownAttackBackgroundImage, cooldownBackgroundAlpha);
        SetAlpha(cooldownBlockingBackgroundImage, cooldownBackgroundAlpha);
    }

    private void OnEnable()
    {
        var playerController = GetComponentInParent<PlayerController>();
        if (playerController)
        {
            id = playerController.ID;
            _playerController = playerController;
        }
    }
    private void SetCooldownColor()
    {
        PlayerManager playerManager = FindFirstObjectByType<PlayerManager>();

        Color playerColor = playerManager.PlayerColors[id];

        cooldownDashBackgroundImage.color = playerColor;
        cooldownAttackBackgroundImage.color = playerColor;
        cooldownBlockingBackgroundImage.color = playerColor;
    }

    private void SetBlockCooldownColor(float strength)
    {
        PlayerManager playerManager = FindFirstObjectByType<PlayerManager>();

        Color playerColor = playerManager.PlayerColors[id];

        cooldownBlockingBackgroundImage.color = playerColor * strength;
    }

    private void Update()
    {
    _dashIsCoolingDown = UpdateCooldown(
        _dashIsCoolingDown,
        CooldownDashImage,
        _playerController.DashCooldown,
        _playerController.dashDelay,
        PlayerState.Dash);

    _attackIsCoolingDown = UpdateCooldown(
        _attackIsCoolingDown,
        CooldownAttackImage,
        _playerController.AttackCooldown,
        _playerController.attackDelay,
        PlayerState.Attack);

    _BlockingIsCoolingDown = UpdateCooldownBlock(
        _BlockingIsCoolingDown,
        CooldownBlockingImage,
        _playerController.BlockCharge,
        _playerController.maxBlockCharge,
        PlayerState.Blocking);
    }

    public void StartDashCooldown(Component sender, object[] obj)
    {
        PlayerController pc = sender as PlayerController;
        if (pc != null && !_dashIsCoolingDown && pc.ID == id)
        {
            _playerController = pc;
            _dashIsCoolingDown = true;

            CooldownDashImage.fillAmount = 1f;
        }
    }
    
    public void StartAttackCooldown(Component sender, object[] obj)
    {
        PlayerController pc = sender as PlayerController;
        if (pc != null && !_attackIsCoolingDown && pc.ID == id)
        {
            _playerController = pc;
            _attackIsCoolingDown = true;

            CooldownAttackImage.fillAmount = 1f;
        }
    }
    public void StartBlockingCooldown(Component sender, object[] obj)
    {
        PlayerController pc = sender as PlayerController;
        if (pc != null && !_BlockingIsCoolingDown && pc.ID == id)
        {
            _playerController = pc;
            _BlockingIsCoolingDown = true;
    
        }
    }
    private bool UpdateCooldownBlock(bool isCoolingDown, Image cooldownImage, float currentCooldown, float maxCooldown, PlayerState state)
    {
        if (!isCoolingDown)
            return false;

        if (_playerController.playerState == state)
        {
            cooldownImage.fillAmount = 1f - (currentCooldown / maxCooldown);
            SetBlockCooldownColor(cooldownBlockInUseColor);
            SetAlpha(cooldownImage, inAction);
        }
        else
        {
            cooldownImage.fillAmount = 1f - (currentCooldown / maxCooldown);
            SetBlockCooldownColor(1f);
            SetAlpha(cooldownImage, coolingDown);
        }

        if (_playerController.CanBlock && currentCooldown >= maxCooldown && _playerController.playerState != state)
        {
            cooldownImage.fillAmount = 0f;

            return false;
        } else if (_playerController.CanBlock && currentCooldown >= maxCooldown * 0.5f && _playerController.playerState != state)
        {
            cooldownImage.fillAmount = 1f - (currentCooldown / maxCooldown);
            SetAlpha(cooldownImage, canUseCooldown);
        }

        return true;
    }
    private bool UpdateCooldown(bool isCoolingDown,Image cooldownImage,float currentCooldown,float maxCooldown,PlayerState state)
    {
        if (!isCoolingDown)
            return false;

        if (_playerController.playerState == state)
        {
            SetAlpha(cooldownImage, inAction);
        }
        else
        {
            cooldownImage.fillAmount = currentCooldown / maxCooldown;
            SetAlpha(cooldownImage, coolingDown);
        }

        if (currentCooldown <= 0 && _playerController.playerState != state)
        {
            cooldownImage.fillAmount = 0f;

            return false;
        }

        return true;
    }
    private void SetAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
