using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OperatorGoldengooseChange : MonoBehaviour
{

    public Slider GameLengthSlider;
    public Slider SecondsPerPointSlider;
    public Slider PickUpDelaySlider;
    public Slider MinBisonUptimeSlider;
    public Slider MaxBisonUptimeSlider;

    public TextMeshProUGUI GameLengthText;
    public TextMeshProUGUI SecondsPerPointText;
    public TextMeshProUGUI PickUpDelayText;
    public TextMeshProUGUI MinBisonUptimeText;
    public TextMeshProUGUI MaxBisonUptimeText;


    
    void Awake()
    {
        GameLengthSlider.value = SettingsManager.Instance.Settings.GGGameLength;
        SecondsPerPointSlider.value = SettingsManager.Instance.Settings.GGSecondsPerPoint;
        PickUpDelaySlider.value = SettingsManager.Instance.Settings.GGPickUpDelay;
        MinBisonUptimeSlider.value = SettingsManager.Instance.Settings.GGMinBisonUptime;
        MaxBisonUptimeSlider.value = SettingsManager.Instance.Settings.GGMaxBisonUptime;
        GameLengthText.text = GameLengthSlider.value.ToString("0.0" + "s");
        SecondsPerPointText.text = SecondsPerPointSlider.value.ToString("0" + "s");
        PickUpDelayText.text = PickUpDelaySlider.value.ToString("0.0" + "s");
        MinBisonUptimeText.text = MinBisonUptimeSlider.value.ToString("0" + "s");
        MaxBisonUptimeText.text = MaxBisonUptimeSlider.value.ToString("0" + "s");
    }

    public void OnGameLengthChanged(float value)
    {
        SettingsManager.Instance.Settings.GGGameLength = value;
        GameLengthText.text = GameLengthSlider.value.ToString("0" + "s");
    }
    public void OnSecondsPerPointChanged(float value)
    {
        SettingsManager.Instance.Settings.GGSecondsPerPoint = value;
        SecondsPerPointText.text = SecondsPerPointSlider.value.ToString("0.0" + "s");
    }
    public void OnPickUpDelayChanged(float value)
    {
        SettingsManager.Instance.Settings.GGPickUpDelay = value;
        PickUpDelayText.text = PickUpDelaySlider.value.ToString("0.0" + "s");
    }
    public void OnMinBisonUptimeChanged(float value)
    {
        SettingsManager.Instance.Settings.GGMinBisonUptime = value;
        MinBisonUptimeText.text = MinBisonUptimeSlider.value.ToString("0" + "s");
    }
    public void OnMaxBisonUptimeChanged(float value)
    {
        SettingsManager.Instance.Settings.GGMaxBisonUptime = value;
        MaxBisonUptimeText.text = MaxBisonUptimeSlider.value.ToString("0" + "s");
    }

}
