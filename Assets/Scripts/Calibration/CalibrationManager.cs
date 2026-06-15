using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CalibrationManager : MonoBehaviour
{
    public GameObject[] BlinkForCalibrations;
    private int controllerCount = 0;

    public void OnCalibration(PlayerInput playerInput)
    {
        if (playerInput.currentActionMap.name == "Operator")
        {
            Debug.Log("found operator");
            return;
        }
        BlinkForCalibrations[controllerCount].SetActive(false);
        controllerCount++;
        if (controllerCount >= BlinkForCalibrations.Length)
        {
            return;
        }
        BlinkForCalibrations[controllerCount].SetActive(true);
    }

    public bool ConfirmCalibration()
    {
        if (controllerCount < BlinkForCalibrations.Length - 1)
        {
            return false;
        }
        SceneManager.LoadScene("SC_Lobby_CO");
        return true;
    }

    public void OnResetCalibration()
    {
        controllerCount = 0;
        foreach (var blink in BlinkForCalibrations)
        {
            blink.SetActive(false);
        }
        BlinkForCalibrations[controllerCount].SetActive(true);
    }
}
