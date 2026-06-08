using MinigameFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCooldownUI : MonoBehaviour
{
    [Header("Cooldown Images Overlay")]
    public Image cooldownDashImage;
    public Image cooldownAttackImage;
    public Image cooldownBlockingImage;
    
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

    private PlayerController _playercontroller;

    private void Start()
    {
        cooldownDashImage.fillAmount = 1f;
        cooldownAttackImage.fillAmount = 1f;
        cooldownBlockingImage.fillAmount = 1f;

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
            _playercontroller = playerController;
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

    private void Update()
    {
    _dashIsCoolingDown = UpdateCooldown(
        _dashIsCoolingDown,
        cooldownDashImage,
        _playercontroller.DashCooldown,
        _playercontroller.dashDelay,
        PlayerState.Dash);

    _attackIsCoolingDown = UpdateCooldown(
        _attackIsCoolingDown,
        cooldownAttackImage,
        _playercontroller.AttackCooldown,
        _playercontroller.attackDelay,
        PlayerState.Attack);

    _BlockingIsCoolingDown = UpdateCooldownBlock(
        _BlockingIsCoolingDown,
        cooldownBlockingImage,
        _playercontroller.BlockCharge,
        _playercontroller.maxBlockCharge,
        PlayerState.Blocking);
    }

    public void StartDashCooldown(Component sender, object[] _obj)
    {
        PlayerController pc = sender as PlayerController;
        if (pc != null && !_dashIsCoolingDown && pc.ID == id)
        {
            _playercontroller = pc;
            _dashIsCoolingDown = true;

            cooldownDashImage.fillAmount = 1f;
        }
    }
    
    public void StartAttackCooldown(Component sender, object[] _obj)
    {
        PlayerController pc = sender as PlayerController;
        if (pc != null && !_attackIsCoolingDown && pc.ID == id)
        {
            _playercontroller = pc;
            _attackIsCoolingDown = true;

            cooldownAttackImage.fillAmount = 1f;
        }
    }
    public void StartBlockingCooldown(Component sender, object[] _obj)
    {
        PlayerController pc = sender as PlayerController;
        if (pc != null && !_BlockingIsCoolingDown && pc.ID == id)
        {
            _playercontroller = pc;
            _BlockingIsCoolingDown = true;
    
        }
    }
    private bool UpdateCooldownBlock(bool isCoolingDown, Image cooldownImage, float currentCooldown, float maxCooldown, PlayerState state)
    {
        if (!isCoolingDown)
            return false;

        if (_playercontroller.playerState == state)
        {
            cooldownImage.fillAmount = 1f - (currentCooldown / maxCooldown);
            SetAlpha(cooldownImage, inAction);
        }
        else
        {
            cooldownImage.fillAmount = 1f - (currentCooldown / maxCooldown);
            SetAlpha(cooldownImage, coolingDown);
        }

        if (_playercontroller.CanBlock && currentCooldown >= maxCooldown && _playercontroller.playerState != state)
        {
            cooldownImage.fillAmount = 0f;

            return false;
        } else if (_playercontroller.CanBlock && currentCooldown >= maxCooldown * 0.5f && _playercontroller.playerState != state)
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

        if (_playercontroller.playerState == state)
        {
            SetAlpha(cooldownImage, inAction);
        }
        else
        {
            cooldownImage.fillAmount = currentCooldown / maxCooldown;
            SetAlpha(cooldownImage, coolingDown);
        }

        if (currentCooldown <= 0 && _playercontroller.playerState != state)
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
