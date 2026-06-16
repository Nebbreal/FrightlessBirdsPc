using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OperatorFrozenfeathersChange : MonoBehaviour
{
    [SerializeField] 
    public Slider PlatformShrinkTargetSlider;
    public Slider LivesSlider;

    public TextMeshProUGUI PlatformShrinkTargetText;
    public TextMeshProUGUI LivesText;

    void Awake()
    {
        PlatformShrinkTargetSlider.value = SettingsManager.Instance.Settings.FFPlatformEndSize;
        LivesSlider.value = SettingsManager.Instance.Settings.FFLives;
        PlatformShrinkTargetText.text = PlatformShrinkTargetSlider.value.ToString("0");
        LivesText.text = LivesSlider.value.ToString("0");
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

}
