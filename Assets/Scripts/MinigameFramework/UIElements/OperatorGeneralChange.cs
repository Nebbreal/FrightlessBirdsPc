using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class OperatorGeneralChange : MonoBehaviour
{
    [FormerlySerializedAs("DashCooldownSlider")] [SerializeField]
    public Slider dashCooldownSlider;
    [FormerlySerializedAs("AttackCooldownSlider")] public Slider attackCooldownSlider;
    [FormerlySerializedAs("BlockDurationSlider")] public Slider blockDurationSlider;
    [FormerlySerializedAs("PlayerAccelerationSlider")] public Slider playerAccelerationSlider;

    [FormerlySerializedAs("DashCooldownText")] public TextMeshProUGUI dashCooldownText;
    [FormerlySerializedAs("AttackCooldownText")] public TextMeshProUGUI attackCooldownText;
    [FormerlySerializedAs("BlockDurationText")] public TextMeshProUGUI blockDurationText;
    [FormerlySerializedAs("PlayerAccelerationText")] public TextMeshProUGUI playerAccelerationText;


    void Awake()
    {
        dashCooldownSlider.value = SettingsManager.Instance.settings.dashCooldown;
        attackCooldownSlider.value = SettingsManager.Instance.settings.attackCooldown;
        blockDurationSlider.value = SettingsManager.Instance.settings.blockCooldown;
        playerAccelerationSlider.value = SettingsManager.Instance.settings.playerMovementSpeed;
        dashCooldownText.text = dashCooldownSlider.value.ToString("0" + "s");
        attackCooldownText.text = attackCooldownSlider.value.ToString("0" + "s");
        blockDurationText.text = blockDurationSlider.value.ToString("0" + "s");
        playerAccelerationText.text = playerAccelerationSlider.value.ToString("0");
    }

    public void OnDashCooldownChanged(float value)
    {
        SettingsManager.Instance.settings.dashCooldown = value;
        dashCooldownText.text = dashCooldownSlider.value.ToString("0" + "s");
    }

    public void OnAttackCooldownChanged(float value)
    {
        SettingsManager.Instance.settings.attackCooldown = value;
        attackCooldownText.text = attackCooldownSlider.value.ToString("0" + "s");
    }

    public void OnMaxBlockDurationChanged(float value)
    {
        SettingsManager.Instance.settings.blockCooldown = value;
        blockDurationText.text = blockDurationSlider.value.ToString("0" + "s");
    }

    public void OnPlayerMovementSpeedChanged(float value)
    {
        SettingsManager.Instance.settings.playerMovementSpeed = value;
        playerAccelerationText.text = playerAccelerationSlider.value.ToString("0");
    }
}
