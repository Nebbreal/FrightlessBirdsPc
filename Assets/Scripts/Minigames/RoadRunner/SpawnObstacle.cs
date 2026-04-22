using UnityEngine;
using System.Collections.Generic;

public class SpawnObstacle : MonoBehaviour
{
    public float minWaitTime;

    public float maxWaitTime;
    public List<GameObject> obstacles;
    public GameObject dangerIndicator;

    public float showDangerBelowTime = 0.5f;
    private float timer = 0f;
    void Start()
    {
        timer = Random.Range(minWaitTime, maxWaitTime) / 10f;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= showDangerBelowTime && !dangerIndicator.activeSelf) {
            dangerIndicator.SetActive(true);
        }
        if (timer <= 0f)
        {
            timer = Random.Range(minWaitTime, maxWaitTime);
            GameObject obst = Instantiate(obstacles[Random.Range(0, obstacles.Count)]);
            obst.transform.position = transform.position;
            obst.transform.rotation = transform.rotation;
            dangerIndicator.SetActive(false);
        }
    }
}
