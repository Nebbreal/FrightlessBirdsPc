using System.Collections;
using EventSystem;
using UnityEngine;

public class CharacterSoundController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip stunSound;
    [SerializeField] private AudioClip braceSound;

    [Header("Attack Pitch Settings")]
    [SerializeField] private float minAttackPitch;
    [SerializeField] private float maxAttackPitch;

    private PlayerState _lastState;

    private void Update()
    {
        if (!playerController) return;

        PlayerState currentState = playerController.playerState;

        if (currentState != _lastState)
        {
            OnStateChanged(currentState);
            _lastState = currentState;
        }
    }

    public void OnPlaySoundClipOnce(Component sender, object[] clipObj)
    {
        if (clipObj == null || clipObj.Length <= 0 || clipObj[0] is not AudioClip clip) return;

        audioSource.PlayOneShot(clip);
    }

    private void OnStateChanged(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Dash:
                audioSource.PlayOneShot(dashSound);
                break;

            case PlayerState.Attack:
                PlayPitchedSound(attackSound, minAttackPitch, maxAttackPitch);
                break;

            case PlayerState.Stunned:
                audioSource.PlayOneShot(stunSound);
                break;
        }
    }
    private void PlayPitchedSound(AudioClip clip, float minPitch, float maxPitch)
    {
        audioSource.clip = clip;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();

        StartCoroutine(ResetPitch(clip.length));
    }
    private IEnumerator ResetPitch(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.pitch = 1f;
    }
}