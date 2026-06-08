using UnityEngine;
using MinigameFramework;

public class PhysicalTimer : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;
    public float rotationSpeed;

    private float timer;
    private float maxTime;

    void Start()
    {
        var _gameConfig = GameManager.Instance.CurrentMinigame;
        maxTime = _gameConfig.roundLength;
        timer = _gameConfig.roundLength;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        var t = timer/maxTime;
        transform.position = Vector3.Lerp(startPos, endPos, 1f - t);
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
