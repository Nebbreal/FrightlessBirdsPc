using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class OperatorFrozenfeathersChange : MonoBehaviour
{
    [SerializeField] 
    public Slider platformShrinkTargetSlider;
    public Slider livesSlider;
    public Slider platformShrinkTimeSlider;

    public TextMeshProUGUI platformShrinkTargetText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI platformShrinkTimeText;

    void Awake()
    {
        platformShrinkTargetSlider.value = SettingsManager.Instance.settings.frozenFeathersPlatformEndSize;
        livesSlider.value = SettingsManager.Instance.settings.frozenFeathersLives;
        platformShrinkTimeSlider.value = SettingsManager.Instance.settings.frozenFeathersPlatformShrinkTime;
        platformShrinkTargetText.text = platformShrinkTargetSlider.value.ToString("0");
        livesText.text = livesSlider.value.ToString("0");
        platformShrinkTimeText.text = platformShrinkTimeSlider.value.ToString("0"+"s");
    }

    public void OnPlatformShrinkTargetChanged(float value)
    {
        SettingsManager.Instance.settings.frozenFeathersPlatformEndSize = value;
        platformShrinkTargetText.text = platformShrinkTargetSlider.value.ToString("0");
    }

    public void OnLivesChanged(float value)
    {
        SettingsManager.Instance.settings.frozenFeathersLives = (int)value;
        livesText.text = livesSlider.value.ToString("0");
    }

    public void OnPlatformShrinkTimeChanged(float value)
    {
        SettingsManager.Instance.settings.frozenFeathersPlatformShrinkTime = value;
        platformShrinkTimeText.text = platformShrinkTimeSlider.value.ToString("0" + "s");
    }
}
