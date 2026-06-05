using System;
using EventSystem;
using UnityEngine;
using UnityEngine.Serialization;

public class EggShield : MonoBehaviour
{
    [SerializeField] 
    private AudioSource shieldAudioSource;
    [SerializeField]
    private AudioClip enterShieldSound;
    [SerializeField] 
    private AudioClip breakShieldSound;
    [SerializeField]
    private SkinnedMeshRenderer shieldRendered;
    [SerializeField] 
    private GameEvent onPlaySoundClipOnce;
    
    public void ShowEggShield()
    {
        shieldRendered.enabled = true;
    }

    public void HideEggShield()
    {
        shieldRendered.enabled = false;
    }

    public void PlayEnterShieldSound()
    {
        shieldAudioSource.PlayOneShot(enterShieldSound);
    }

    public void PlayBreakShieldSound()
    {
        shieldAudioSource.PlayOneShot(breakShieldSound);
    }
}
