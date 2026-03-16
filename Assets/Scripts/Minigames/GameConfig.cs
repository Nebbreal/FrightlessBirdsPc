using UnityEngine;
using UnityEngine.InputSystem;

public class GameConfig : MonoBehaviour
{
    [Header("Round")]
    [Tooltip("In seconds")]
    public float roundLength;
    public int playerLives;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRoundInput(InputAction.CallbackContext context)
    {
        
    }

    public void DashInput(InputAction.CallbackContext context)
    {
        
    }
}
