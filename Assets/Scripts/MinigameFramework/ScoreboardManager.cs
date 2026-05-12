using System;
using System.Collections.Generic;
using System.Linq;
using EventSystem;
using Minigames;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ScoreboardManager : MonoBehaviour
{
    [NonSerialized] public Dictionary<string, int> PlayerScores = new();
    public GameObject crownUI;
    public float outwardDistance;

    private List<GameObject> _instantiatedCrowns = new List<GameObject>();

    public void GainPoint(Component sender, object[] winnerObjs)
    {
        if (winnerObjs == null || winnerObjs.Length == 0) return;
        foreach (GameObject player in winnerObjs)
        {
            PlayerScores[player.name]++;
            GameObject crown = Instantiate(crownUI);
            crown.SetActive(false);
            crown.transform.rotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward);
            crown.transform.position += crown.transform.forward * outwardDistance;
            _instantiatedCrowns.Add(crown);
        }
    }

    public void OnCleanScoreboardUI(Component sender, object[] data)
    {
        foreach (GameObject crown in _instantiatedCrowns)
        {
            Destroy(crown);
        }
        _instantiatedCrowns.Clear();
    }

    public void OnUpdateScoreboard(Component sender, object[] modelsObj)
    {
        foreach (GameObject crown in _instantiatedCrowns)
        {
            crown.SetActive(true);
        }
    }
}
