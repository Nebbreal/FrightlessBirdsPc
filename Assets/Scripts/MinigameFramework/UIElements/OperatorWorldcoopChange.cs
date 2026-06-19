using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class OperatorWorldcoopChange : MonoBehaviour
{
    [FormerlySerializedAs("GameLengthSlider")] [SerializeField]
    public Slider gameLengthSlider;
    [FormerlySerializedAs("GoalsToWinSlider")] public Slider goalsToWinSlider;
    [FormerlySerializedAs("GameLengthText")] public TextMeshProUGUI gameLengthText;
    [FormerlySerializedAs("GoalsToWinText")] public TextMeshProUGUI goalsToWinText;

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
