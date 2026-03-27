using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CalibrationManager : MonoBehaviour
{
    public GameObject[] BlinkForCalibrations;
    private int controllerCount = 0;

    public void OnCalibration(PlayerInput playerInput)
    {
        BlinkForCalibrations[controllerCount].SetActive(false);
        controllerCount++;
        if (controllerCount >= BlinkForCalibrations.Length)
        {
            return;
        }
        BlinkForCalibrations[controllerCount].SetActive(true);
    }

    public void ConfirmCalibration()
    {
        if (controllerCount < BlinkForCalibrations.Length - 1)
        {
            return;
        }
        SceneManager.LoadScene("MainScene");
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
