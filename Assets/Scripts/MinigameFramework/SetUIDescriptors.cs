using MinigameFramework;
using Minigames;
using TMPro;
using UnityEngine;

public class SetUIDescriptors : MonoBehaviour
{
    public TextMeshProUGUI[] titles;
    public TextMeshProUGUI[] descriptions;

    public void SetDescriptorUI(Component sender, object[] obj)
    {
        Minigame config = GameManager.Instance.CurrentMinigame;

        foreach (TextMeshProUGUI title in titles)
        {
            title.text = config.gameName;
        }
        foreach (TextMeshProUGUI description in descriptions)
        {
            description.text = config.gameDescription;
        }
    }
}
