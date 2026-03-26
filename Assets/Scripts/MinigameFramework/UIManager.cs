using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    [SerializeField] 
    private GameObject winTextObject;
    [FormerlySerializedAs("winningPlayerText")] [SerializeField]
    private TextMeshProUGUI winningPlayerTextTop;
    [SerializeField]
    private TextMeshProUGUI winningPlayerTextBottom;
    
    public void UpdateWinningPlayer(string winningPlayer)
    {
        winningPlayerTextTop.text = winningPlayer + " wins!";
        winningPlayerTextBottom.text = winningPlayer + " wins!";
        winTextObject.SetActive(true);
    }

    public void SetWinUI(bool state)
    {
        winTextObject.SetActive(state);
    }
}
