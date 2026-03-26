using UnityEngine;
using UnityEngine.InputSystem;

public class GameConfig : MonoBehaviour
{
    [Header("Round")]
    [Tooltip("In seconds")]
    public float roundLength;
    public int playerLives;
    [Tooltip("In seconds")] 
    public float countdownLength = 5;
}
