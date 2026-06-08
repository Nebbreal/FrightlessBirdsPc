using UnityEngine;
using MinigameFramework;

public class PhysicalTimer2 : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;

    private float _elapsedTime;
    private float _maxTime;
    private Quaternion _startRotation;

    void Start()
    {
        var gameConfig = GameManager.Instance.CurrentMinigame;
        _maxTime = gameConfig.roundLength;
        _startRotation = transform.localRotation;
        _elapsedTime = 0f;
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;

        var t = _maxTime > 0f ? Mathf.Clamp01(_elapsedTime / _maxTime) : 1f;
        transform.localPosition = Vector3.Lerp(startPos, endPos, t);
        transform.localRotation = _startRotation * Quaternion.Euler(0f, 360f * t, 0f);
    }
}
