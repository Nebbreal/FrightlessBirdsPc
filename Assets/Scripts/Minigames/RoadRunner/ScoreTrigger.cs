using UnityEngine;
using EventSystem;
using Minigames;
using Minigames.RoadRunner;

public class ScoreTrigger : MonoBehaviour
{
    public GameEvent onScorePoint;
    public int side = 0;
    public RoadRunnerMinigameManager Manager;
    private bool _hasEnded;

    public void Start()
    {
        _hasEnded = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (_hasEnded) return;

        if (side != Manager.playerSides[other.gameObject.name])
        {
            onScorePoint.Raise(this, other.gameObject);
            Manager.playerSides[other.gameObject.name] = side;
        }
    }
}
