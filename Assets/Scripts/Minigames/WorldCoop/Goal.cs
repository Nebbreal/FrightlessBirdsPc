using UnityEngine;
using EventSystem;

public class Goal : MonoBehaviour
{
    [SerializeField] private GameEvent onGoalScored;
    [SerializeField] private int teamID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            onGoalScored.Raise(this, new object[] { teamID });
        }
    }
}
