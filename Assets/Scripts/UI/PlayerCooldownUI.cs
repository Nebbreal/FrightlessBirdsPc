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
        if (_dashIsCoolingDown)
        {       
            if (_playercontroller.playerState == PlayerState.Dash)
            {
                SetAlpha(cooldownDashImage, 0.9f);
            }
            else
            {
                cooldownDashImage.fillAmount = _playercontroller._dashCooldown / _playercontroller.dashDelay;
                SetAlpha(cooldownDashImage, 0.85f);
            }

            if (_playercontroller._dashCooldown <= 0 && _playercontroller.playerState != PlayerState.Dash)
            {
                _dashIsCoolingDown = false;
                cooldownDashImage.fillAmount = 0f;

                SetAlpha(cooldownDashImage, 0f);
            }
        }
        if (_attackIsCoolingDown)
        {       
            if (_playercontroller.playerState == PlayerState.Attack)
            {
                SetAlpha(cooldownAttackImage, 0.9f);
            }
            else
            {
                cooldownAttackImage.fillAmount = _playercontroller._attackCooldown / _playercontroller.attackDelay;
                SetAlpha(cooldownAttackImage, 0.85f);
            }

            if (_playercontroller._attackCooldown <= 0 && _playercontroller.playerState != PlayerState.Attack)
            {
                _attackIsCoolingDown = false;
                cooldownAttackImage.fillAmount = 0f;

                SetAlpha(cooldownAttackImage, 0f);
            }
        }
        if (_bracedIsCoolingDown)
        {       
            if (_playercontroller.playerState == PlayerState.Braced)
            {
                SetAlpha(cooldownBracedImage, 0.9f);
            }
            else
            {
                cooldownBracedImage.fillAmount = _playercontroller._bracedCooldown / _playercontroller.bracedDelay;
                SetAlpha(cooldownBracedImage, 0.85f);
            }

            if (_playercontroller._bracedCooldown <= 0 && _playercontroller.playerState != PlayerState.Braced)
            {
                _bracedIsCoolingDown = false;
                cooldownBracedImage.fillAmount = 0f;
                
                SetAlpha(cooldownBracedImage, 0f);
            }
        }
    }

    public void StartDashCooldown(Component sender, object[] _obj)
    {
        PlayerController pc = sender as PlayerController;
        if (!_dashIsCoolingDown && pc.ID == id)
        {
            _playercontroller = pc;
            _dashIsCoolingDown = true;

            cooldownDashImage.fillAmount = 1f;
        }
    }
    public void StartAttackCooldown(Component sender, object[] _obj)
    {
        PlayerController pc = sender as PlayerController;
        if (!_attackIsCoolingDown && pc.ID == id)
        {
            _playercontroller = pc;
            _attackIsCoolingDown = true;

            cooldownAttackImage.fillAmount = 1f;
        }
    }
    public void StartBracedCooldown(Component sender, object[] _obj)
    {
        PlayerController pc = sender as PlayerController;
        if (!_bracedIsCoolingDown && pc.ID == id)
        {
            _playercontroller = pc;
            _bracedIsCoolingDown = true;

            cooldownBracedImage.fillAmount = 1f;        }
    }
    private void SetAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
