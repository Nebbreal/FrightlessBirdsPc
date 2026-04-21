using UnityEngine;
using System.Collections.Generic;

public class SpawnObstacle : MonoBehaviour
{
    public float minWaitTime;

    public float maxWaitTime;
    public List<GameObject> obstacles;
    public GameObject dangerIndicator;

    private float timer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = Random.Range(minWaitTime, maxWaitTime) / 10f;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.5f && !dangerIndicator.activeSelf) {
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
