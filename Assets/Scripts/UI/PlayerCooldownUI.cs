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
    private bool _dashIsCoolingDown;
    private bool _attackIsCoolingDown;
    private bool _bracedIsCoolingDown;

    [Header("Cooldown Background Images")]
    public Image cooldownDashBackgroundImage;
    public Image cooldownAttackBackgroundImage;
    public Image cooldownBracedBackgroundImage;
    public float cooldownBackgroundAlpha = 0.8f;

    private PlayerController _playercontroller;

    private void Start()
    {
        cooldownDashImage.fillAmount = 1f;
        cooldownAttackImage.fillAmount = 1f;
        cooldownBracedImage.fillAmount = 1f;

        SetCooldownColor();

        SetAlpha(cooldownDashBackgroundImage, cooldownBackgroundAlpha);
        SetAlpha(cooldownAttackBackgroundImage, cooldownBackgroundAlpha);
        SetAlpha(cooldownBracedBackgroundImage, cooldownBackgroundAlpha);
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
        cooldownBracedBackgroundImage.color = playerColor;
    }

    private void Update()
    {
    _dashIsCoolingDown = UpdateCooldown(
        _dashIsCoolingDown,
        cooldownDashImage,
        _playercontroller._dashCooldown,
        _playercontroller.dashDelay,
        PlayerState.Dash);

    _attackIsCoolingDown = UpdateCooldown(
        _attackIsCoolingDown,
        cooldownAttackImage,
        _playercontroller._attackCooldown,
        _playercontroller.attackDelay,
        PlayerState.Attack);

    _bracedIsCoolingDown = UpdateCooldown(
        _bracedIsCoolingDown,
        cooldownBracedImage,
        _playercontroller._bracedCooldown,
        _playercontroller.bracedDelay,
        PlayerState.Braced);
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
    public void StartBracedCooldown(Component sender, object[] _obj)
    {
        PlayerController pc = sender as PlayerController;
        if (pc != null && !_bracedIsCoolingDown && pc.ID == id)
        {
            _playercontroller = pc;
            _bracedIsCoolingDown = true;

            cooldownBracedImage.fillAmount = 1f;        
        }
    }
    private bool UpdateCooldown(bool isCoolingDown,Image cooldownImage,float currentCooldown,float maxCooldown,PlayerState state)
    {
        if (!isCoolingDown)
            return false;

        if (_playercontroller.playerState == state)
        {
            SetAlpha(cooldownImage, 0.9f);
        }
        else
        {
            cooldownImage.fillAmount = currentCooldown / maxCooldown;
            SetAlpha(cooldownImage, 0.85f);
        }

        if (currentCooldown <= 0 && _playercontroller.playerState != state)
        {
            cooldownImage.fillAmount = 0f;
            SetAlpha(cooldownImage, 0f);

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
