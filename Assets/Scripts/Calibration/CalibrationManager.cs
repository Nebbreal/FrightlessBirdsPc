using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CalibrationManager : MonoBehaviour
{
    [FormerlySerializedAs("BlinkForCalibrations")] public GameObject[] blinkForCalibrations;
    private int _controllerCount = 0;

    public void OnCalibration(PlayerInput playerInput)
    {
        if (playerInput.currentActionMap.name == "Operator") return;
        blinkForCalibrations[_controllerCount].SetActive(false);
        _controllerCount++;
        if (_controllerCount >= blinkForCalibrations.Length)
        {
            return;
        }
        blinkForCalibrations[_controllerCount].SetActive(true);
    }

    public bool ConfirmCalibration()
    {
        if (_controllerCount < blinkForCalibrations.Length - 1)
        {
            return false;
        }
        SceneManager.LoadScene("SC_Lobby_CO");
        return true;
    }

    public void OnResetCalibration()
    {
        _controllerCount = 0;
        foreach (var blink in blinkForCalibrations)
        {
            blink.SetActive(false);
        }
        blinkForCalibrations[_controllerCount].SetActive(true);
    }
}
