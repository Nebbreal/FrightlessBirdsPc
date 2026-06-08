using UnityEngine;

public class TestBots : MonoBehaviour
{
    public PlayerState targetState;
    public PlayerController player;

    void LateUpdate()
    {
        player.MoveInput = Vector3.zero;
        // Target state is the state that a bot tries to keep
        if (targetState != PlayerState.Idle && player.playerState is PlayerState.Idle or PlayerState.Movement)
        {
            if (targetState is PlayerState.Attack)
            {
                if (player.AttackCooldown <= 0f)
                {
                    player.DoAttack = true;
                }
                else
                {
                    return;
                }
            }
            else if (targetState is PlayerState.Dash)
            {
                if (player.DashCooldown <= 0f)
                {
                    if (player.LastMoveDirection.x == 1f)
                    {
                        player.LastMoveDirection = Vector3.left;
                    }
                    else
                    {
                        player.LastMoveDirection = Vector3.right;
                    }

                    player.DoDash = true;
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
