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
            SceneManager.LoadScene("Main");
            return;
        }
        BlinkForCalibrations[controllerCount].SetActive(true);
    }
}
