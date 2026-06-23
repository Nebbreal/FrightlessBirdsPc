using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OperatorWorldcoopChange : MonoBehaviour
{
    [SerializeField]
    public Slider gameLengthSlider;
    public Slider goalsToWinSlider;
    public TextMeshProUGUI gameLengthText;
    public TextMeshProUGUI goalsToWinText;

    void Awake()
    {
        gameLengthSlider.value = SettingsManager.Instance.settings.worldCoopGameLength;
        goalsToWinSlider.value = SettingsManager.Instance.settings.worldCoopGoalsToWin;
        gameLengthText.text = gameLengthSlider.value.ToString("0" + "s");
        goalsToWinText.text = goalsToWinSlider.value.ToString("0");
    }

    public void OnGameLengthChanged(float value)
    {
        SettingsManager.Instance.settings.worldCoopGameLength = value;
        gameLengthText.text = gameLengthSlider.value.ToString("0" + "s");
    }

    public void OnGoalsToWinChanged(float value)
    {
        SettingsManager.Instance.settings.worldCoopGoalsToWin = (int)value;
        goalsToWinText.text = goalsToWinSlider.value.ToString("0");

    }
}
