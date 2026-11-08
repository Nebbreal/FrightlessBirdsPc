using System.Linq;
using MinigameFramework;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject[] playerUIs;
    [Tooltip("Optional, only set when applicable")]
    public GameObject playerUIsParent;
 
    public void OnPlayerJoinEnableUI(Component sender, object[] number)
    {
        if (number.First() is int id)
        {
            playerUIs[id].SetActive(true);
        }
    }

    public void OnPlayerJoinDisableUI(Component sender, object[] number)
    {
        if (number.First() is int id)
        {
            playerUIs[id].SetActive(false);
        }
    }

    public void OnLoadingScreenFinishedLateEnableUIs(Component sender, object[] _)
    {
        if (GameManager.Instance.IsMinigameSceneActive) return;
        playerUIsParent.SetActive(true);
    }
    
}
