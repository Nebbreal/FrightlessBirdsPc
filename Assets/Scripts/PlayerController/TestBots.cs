using UnityEngine;

public class TestBots : MonoBehaviour
{
    public PlayerState targetState;
    public PlayerController player;

    void LateUpdate()
    {
        player._moveInput = Vector3.zero;
        // Target state is the state that a bot tries to keep
        if (targetState != PlayerState.Idle && player.playerState is PlayerState.Idle or PlayerState.Movement)
        {
            if (targetState is PlayerState.Attack)
            {
                if (player._attackCooldown <= 0f)
                {
                    player._doAttack = true;
                }
                else
                {
                    return;
                }
            }
            else if (targetState is PlayerState.Dash)
            {
                if (player._dashCooldown <= 0f)
                {
                    if (player._lastMoveDirection.x == 1f)
                    {
                        player._lastMoveDirection = Vector3.left;
                    }
                    else
                    {
                        player._lastMoveDirection = Vector3.right;
                    }

                    player._doDash = true;
                }
                else
                {
                    return;
                }
            }

            player.SetPlayerState(targetState);
        }
    }
}
