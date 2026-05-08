using System.Linq;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject[] playerUIs;

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
}
