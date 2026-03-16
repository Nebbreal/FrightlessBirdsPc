using System;
using UnityEngine;

public class WaterCollision : MonoBehaviour
{
    public AudioSource splashSource;
    public LayerMask lMask;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != lMask)
        {
            splashSource.Play();

        }
    }
}
