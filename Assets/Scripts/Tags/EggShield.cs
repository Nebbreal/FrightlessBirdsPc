using System;
using UnityEngine;
using UnityEngine.Serialization;

public class EggShield : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer shieldRendered;
    public void ShowEggShield()
    {
        Debug.Log("Showing Egg Shield");
        shieldRendered.enabled = true;
    }

    public void HideEggShield()
    {
        Debug.Log("Hiding Egg Shield");
        shieldRendered.enabled = false;
    }
}
