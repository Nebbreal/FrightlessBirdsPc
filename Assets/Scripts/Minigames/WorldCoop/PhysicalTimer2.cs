using UnityEngine;
using MinigameFramework;

public class PhysicalTimer2 : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;

    private float elapsedTime;
    private float maxTime;
    private Quaternion startRotation;

    void Start()
    {
        var _gameConfig = GameManager.Instance.CurrentMinigame;
        maxTime = _gameConfig.roundLength;
        startRotation = transform.localRotation;
        elapsedTime = 0f;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        var t = maxTime > 0f ? Mathf.Clamp01(elapsedTime / maxTime) : 1f;
        transform.localPosition = Vector3.Lerp(startPos, endPos, t);
        transform.localRotation = startRotation * Quaternion.Euler(0f, 360f * t, 0f);
    }
}
