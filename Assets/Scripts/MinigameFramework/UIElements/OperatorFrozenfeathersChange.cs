using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class OperatorFrozenfeathersChange : MonoBehaviour
{
    [FormerlySerializedAs("PlatformShrinkTargetSlider")] [SerializeField] 
    public Slider platformShrinkTargetSlider;
    [FormerlySerializedAs("LivesSlider")] public Slider livesSlider;
    [FormerlySerializedAs("PlatformShrinkTimeSlider")] public Slider platformShrinkTimeSlider;

    [FormerlySerializedAs("PlatformShrinkTargetText")] public TextMeshProUGUI platformShrinkTargetText;
    [FormerlySerializedAs("LivesText")] public TextMeshProUGUI livesText;
    [FormerlySerializedAs("PlatformShrinkTimeText")] public TextMeshProUGUI platformShrinkTimeText;

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
