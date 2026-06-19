using UnityEngine;
using System.Collections.Generic;

public class SpawnObstacle : MonoBehaviour
{
    public float minWaitTime;

    public float maxWaitTime;
    public List<GameObject> obstacles;
    public List<GameObject> possibleSpawnPositions;
    public List<GameObject> dangerIndicators;

    public float showDangerBelowTime = 0.5f;
    private int _currentPos;
    private float _timer = 0f;
    void Start()
    {
        _timer = Random.Range(minWaitTime, maxWaitTime);
    }

    public void LoadFromSettings()
    {
        minWaitTime = SettingsManager.Instance.settings.goldenGooseMinBisonUptime;
        maxWaitTime = SettingsManager.Instance.settings.goldenGooseMaxBisonUptime;
    }

    private void Awake()
    {
        LoadFromSettings();
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= showDangerBelowTime && !dangerIndicators[_currentPos].activeSelf) {
            dangerIndicators[_currentPos].SetActive(true);
        }
        if (_timer <= 0f)
        {
            _timer = Random.Range(minWaitTime, maxWaitTime);
            GameObject obst = Instantiate(obstacles[Random.Range(0, obstacles.Count)]);
            obst.transform.position = possibleSpawnPositions[_currentPos].transform.position;
            obst.transform.rotation = possibleSpawnPositions[_currentPos].transform.rotation;
            dangerIndicators[_currentPos].SetActive(false);
            _currentPos = Random.Range(0, possibleSpawnPositions.Count);
        }
    }
}
