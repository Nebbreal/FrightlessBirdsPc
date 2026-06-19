using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class OperatorGoldengooseChange : MonoBehaviour
{
    [FormerlySerializedAs("GameLengthSlider")] public Slider gameLengthSlider;
    [FormerlySerializedAs("SecondsPerPointSlider")] public Slider secondsPerPointSlider;
    [FormerlySerializedAs("PickUpDelaySlider")] public Slider pickUpDelaySlider;
    [FormerlySerializedAs("MinBisonUptimeSlider")] public Slider minBisonUptimeSlider;
    [FormerlySerializedAs("MaxBisonUptimeSlider")] public Slider maxBisonUptimeSlider;

    [FormerlySerializedAs("GameLengthText")] public TextMeshProUGUI gameLengthText;
    [FormerlySerializedAs("SecondsPerPointText")] public TextMeshProUGUI secondsPerPointText;
    [FormerlySerializedAs("PickUpDelayText")] public TextMeshProUGUI pickUpDelayText;
    [FormerlySerializedAs("MinBisonUptimeText")] public TextMeshProUGUI minBisonUptimeText;
    [FormerlySerializedAs("MaxBisonUptimeText")] public TextMeshProUGUI maxBisonUptimeText;


    
    void Awake()
    {
        gameLengthSlider.value = SettingsManager.Instance.settings.goldenGooseGameLength;
        secondsPerPointSlider.value = SettingsManager.Instance.settings.goldenGooseSecondsPerPoint;
        pickUpDelaySlider.value = SettingsManager.Instance.settings.goldenGoosePickUpDelay;
        minBisonUptimeSlider.value = SettingsManager.Instance.settings.goldenGooseMinBisonUptime;
        maxBisonUptimeSlider.value = SettingsManager.Instance.settings.goldenGooseMaxBisonUptime;
        gameLengthText.text = gameLengthSlider.value.ToString("0" + "s");
        secondsPerPointText.text = secondsPerPointSlider.value.ToString("0" + "s");
        pickUpDelayText.text = pickUpDelaySlider.value.ToString("0.0" + "s");
        minBisonUptimeText.text = minBisonUptimeSlider.value.ToString("0" + "s");
        maxBisonUptimeText.text = maxBisonUptimeSlider.value.ToString("0" + "s");
    }

    public void OnGameLengthChanged(float value)
    {
        SettingsManager.Instance.settings.goldenGooseGameLength = value;
        gameLengthText.text = gameLengthSlider.value.ToString("0" + "s");
    }
    public void OnSecondsPerPointChanged(float value)
    {
        SettingsManager.Instance.settings.goldenGooseSecondsPerPoint = value;
        secondsPerPointText.text = secondsPerPointSlider.value.ToString("0.0" + "s");
    }
    public void OnPickUpDelayChanged(float value)
    {
        SettingsManager.Instance.settings.goldenGoosePickUpDelay = value;
        pickUpDelayText.text = pickUpDelaySlider.value.ToString("0.0" + "s");
    }
    public void OnMinBisonUptimeChanged(float value)
    {
        SettingsManager.Instance.settings.goldenGooseMinBisonUptime = value;
        minBisonUptimeText.text = minBisonUptimeSlider.value.ToString("0" + "s");
    }
    public void OnMaxBisonUptimeChanged(float value)
    {
        SettingsManager.Instance.settings.goldenGooseMaxBisonUptime = value;
        maxBisonUptimeText.text = maxBisonUptimeSlider.value.ToString("0" + "s");
    }

}
