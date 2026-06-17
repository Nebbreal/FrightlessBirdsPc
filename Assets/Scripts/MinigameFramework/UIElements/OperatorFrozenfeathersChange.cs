using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OperatorFrozenfeathersChange : MonoBehaviour
{
    [SerializeField] 
    public Slider PlatformShrinkTargetSlider;
    public Slider LivesSlider;
    public Slider PlatformShrinkTimeSlider;

    public TextMeshProUGUI PlatformShrinkTargetText;
    public TextMeshProUGUI LivesText;
    public TextMeshProUGUI PlatformShrinkTimeText;

    void Awake()
    {
        PlatformShrinkTargetSlider.value = SettingsManager.Instance.Settings.FFPlatformEndSize;
        LivesSlider.value = SettingsManager.Instance.Settings.FFLives;
        PlatformShrinkTimeSlider.value = SettingsManager.Instance.Settings.FFPlatformShrinkTime;
        PlatformShrinkTargetText.text = PlatformShrinkTargetSlider.value.ToString("0");
        LivesText.text = LivesSlider.value.ToString("0");
        PlatformShrinkTimeText.text = PlatformShrinkTimeSlider.value.ToString("0"+"s");
    }

    public void OnPlatformShrinkTargetChanged(float value)
    {
        SettingsManager.Instance.Settings.FFPlatformEndSize = value;
        PlatformShrinkTargetText.text = PlatformShrinkTargetSlider.value.ToString("0");
    }

    public void OnLivesChanged(float value)
    {
        SettingsManager.Instance.Settings.FFLives = (int)value;
        LivesText.text = LivesSlider.value.ToString("0");
    }

    public void OnPlatformShrinkTimeChanged(float value)
    {
        SettingsManager.Instance.Settings.FFPlatformShrinkTime = value;
        PlatformShrinkTimeText.text = PlatformShrinkTimeSlider.value.ToString("0" + "s");
    }
}
