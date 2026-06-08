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
    private int currentPos;
    private float timer = 0f;
    void Start()
    {
        timer = Random.Range(minWaitTime, maxWaitTime);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= showDangerBelowTime && !dangerIndicators[currentPos].activeSelf) {
            dangerIndicators[currentPos].SetActive(true);
        }
        if (timer <= 0f)
        {
            timer = Random.Range(minWaitTime, maxWaitTime);
            GameObject obst = Instantiate(obstacles[Random.Range(0, obstacles.Count)]);
            obst.transform.position = possibleSpawnPositions[currentPos].transform.position;
            obst.transform.rotation = possibleSpawnPositions[currentPos].transform.rotation;
            dangerIndicators[currentPos].SetActive(false);
            currentPos = Random.Range(0, possibleSpawnPositions.Count);
        }
    }
}
