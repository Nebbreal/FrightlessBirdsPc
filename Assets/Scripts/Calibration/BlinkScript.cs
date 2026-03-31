using UnityEngine;
using UnityEngine.UI;

public class BlinkScript : MonoBehaviour
{
    private float blinkTimer = 0f;
    public float blinkDuration = 0.5f; // Duration of each blink
    public Image image; // Reference to the Image component
    
    void Update()
    {
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkDuration)
        {
            image.enabled = !image.enabled; // Toggle visibility
            blinkTimer = 0f;
        }
    }
}
