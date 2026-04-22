using UnityEngine;
using EventSystem;
using Minigames;
using Minigames.RoadRunner;

public class ScoreTrigger : MonoBehaviour
{
    public GameEvent onScorePoint;
    public int side = 0;
    public RoadRunnerMinigameManager Manager;

    public void OnTriggerEnter(Collider other)
    {
        if (side != Manager.playerSides[other.gameObject.name])
        {
            onScorePoint.Raise(this, other.gameObject);
            Manager.playerSides[other.gameObject.name] = side;
        }
    }
}
