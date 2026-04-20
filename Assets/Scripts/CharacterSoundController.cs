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

    private PlayerState lastState;

    private void Update()
    {
        if (playerController == null) return;

        PlayerState currentState = playerController.playerState;

        if (currentState != lastState)
        {
            OnStateChanged(currentState);
            lastState = currentState;
        }
    }

    private void OnStateChanged(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Dash:
                audioSource.PlayOneShot(dashSound);
                break;

            case PlayerState.Attack:
                audioSource.PlayOneShot(attackSound);
                break;

            case PlayerState.Stunned:
                audioSource.PlayOneShot(stunSound);
                break;

            case PlayerState.Braced:
                audioSource.PlayOneShot(braceSound);
                break;
        }
    }
}