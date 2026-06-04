using System;
using UnityEngine;
using UnityEngine.Serialization;

public class EggShield : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer shieldRendered;
    public void ShowEggShield()
    {
        shieldRendered.enabled = true;
    }

    public void HideEggShield()
    {
        shieldRendered.enabled = false;
    }
}
