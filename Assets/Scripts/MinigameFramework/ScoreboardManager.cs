using System;
using System.Collections.Generic;
using System.Linq;
using EventSystem;
using MinigameFramework;
using MinigameFramework.UIElements;
using Minigames;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ScoreboardManager : MonoBehaviour
{
    [NonSerialized] public List<int> PlayerScores = new();
    public List<GameObject> PlayerScoreTexts = new();
    public GameObject elements;
    public GameObject crownUI;
    public float outwardDistance;

    private List<GameObject> _instantiatedCrowns = new List<GameObject>();

    private void Awake()
    {
        ResetPlayerScores();
    }

    public void ResetPlayerScores()
    {
        PlayerScores = new List<int> { 0, 0, 0, 0 };
    }

    public bool showScores;
    public bool spawnCrown;

    void Update()
    {
        if (spawnCrown)
            ButtonFunction1();
        if (showScores)
            elements.SetActive(true);
        spawnCrown = false;
        showScores = false;

    }

    void ButtonFunction1()
    {
        int id = 0;
        PlayerScores[id]++;
        GameObject crown = Instantiate(crownUI, elements.transform);
        crown.transform.rotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward);
        crown.transform.position += crown.transform.up * outwardDistance;
        ScoreboardCrown c = crown.GetComponent<ScoreboardCrown>();
        c.target = PlayerScoreTexts[id].gameObject;
        _instantiatedCrowns.Add(crown);
    }

    public void GainPoint(Component sender, object[] winnerObjs)
    {
        if (winnerObjs == null || winnerObjs.Length == 0) return;
        foreach (GameObject player in winnerObjs)
        {
            PlayerController p = player.GetComponent<PlayerController>();
            PlayerScores[p.ID]++;
            GameObject crown = Instantiate(crownUI, elements.transform);
            crown.transform.rotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward);
            crown.transform.position += crown.transform.up * outwardDistance;
            ScoreboardCrown c = crown.GetComponent<ScoreboardCrown>();
            c.target = PlayerScoreTexts[p.ID].gameObject;
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
        GameManager.Instance.ToLobby();
    }

    public void OnUpdateScoreboard(Component sender, object[] modelsObj)
    {
        foreach (GameObject crown in _instantiatedCrowns)
        {
            crown.SetActive(true);
        }
    }
}
