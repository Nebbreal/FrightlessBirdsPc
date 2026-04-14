using UnityEngine;

public class RoadRunnerCar : MonoBehaviour
{
    public Vector3 direction;
    public float resetLength;
    private Vector3 start;

    void Start()
    {
        start = transform.position;
    }

    void Update()
    {
        transform.position += direction;

        if ((transform.position - start).magnitude > resetLength)
        {
            transform.position = start;
        }
    }
}
