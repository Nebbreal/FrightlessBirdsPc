using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class OperatorGeneralChange : MonoBehaviour
{
    [SerializeField]
    public Slider DashCooldownSlider;
    public Slider AttackCooldownSlider;
    public Slider BlockDurationSlider;
    public Slider PlayerAccelerationSlider;

    public TextMeshProUGUI DashCooldownText;
    public TextMeshProUGUI AttackCooldownText;
    public TextMeshProUGUI BlockDurationText;
    public TextMeshProUGUI PlayerAccelerationText;


    void Awake()
    {
        DashCooldownSlider.value = SettingsManager.Instance.Settings.DashCooldown;
        AttackCooldownSlider.value = SettingsManager.Instance.Settings.AttackCooldown;
        BlockDurationSlider.value = SettingsManager.Instance.Settings.BlockCooldown;
        PlayerAccelerationSlider.value = SettingsManager.Instance.Settings.PlayerMovementSpeed;
        DashCooldownText.text = DashCooldownSlider.value.ToString("0" + "s");
        AttackCooldownText.text = AttackCooldownSlider.value.ToString("0" + "s");
        BlockDurationText.text = BlockDurationSlider.value.ToString("0" + "s");
        PlayerAccelerationText.text = PlayerAccelerationSlider.value.ToString("0");
    }

    public void OnDashCooldownChanged(float value)
    {
        SettingsManager.Instance.Settings.DashCooldown = value;
        DashCooldownText.text = DashCooldownSlider.value.ToString("0" + "s");
    }

    public void OnAttackCooldownChanged(float value)
    {
        SettingsManager.Instance.Settings.AttackCooldown = value;
        AttackCooldownText.text = AttackCooldownSlider.value.ToString("0" + "s");
    }

    public void OnMaxBlockDurationChanged(float value)
    {
        SettingsManager.Instance.Settings.BlockCooldown = value;
        BlockDurationText.text = BlockDurationSlider.value.ToString("0" + "s");
    }

    public void OnPlayerMovementSpeedChanged(float value)
    {
        SettingsManager.Instance.Settings.PlayerMovementSpeed = value;
        PlayerAccelerationText.text = PlayerAccelerationSlider.value.ToString("0");
    }
}
