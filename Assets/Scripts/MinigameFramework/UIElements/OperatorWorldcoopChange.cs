using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OperatorWorldcoopChange : MonoBehaviour
{
    [SerializeField]
    public Slider GameLengthSlider;
    public Slider GoalsToWinSlider;
    public TextMeshProUGUI GameLengthText;
    public TextMeshProUGUI GoalsToWinText;

    void Awake()
    {
        GameLengthSlider.value = SettingsManager.Instance.Settings.WCGameLength;
        GoalsToWinSlider.value = SettingsManager.Instance.Settings.WCGoalsToWin;
        GameLengthText.text = GameLengthSlider.value.ToString("0" + "s");
        GoalsToWinText.text = GoalsToWinSlider.value.ToString("0");
    }

    public void OnGameLengthChanged(float value)
    {
        SettingsManager.Instance.Settings.WCGameLength = value;
        GameLengthText.text = GameLengthSlider.value.ToString("0" + "s");
    }

    public void OnGoalsToWinChanged(float value)
    {
        SettingsManager.Instance.Settings.WCGoalsToWin = (int)value;
        GoalsToWinText.text = GoalsToWinSlider.value.ToString("0");

    }
}
