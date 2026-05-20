using EventSystem;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardPanel : MonoBehaviour
{
    public GameEvent onShowCrowns;
    public float animationDuration;
    private float _animationTime = 0f;
    private float width = 0f;

    private void OnEnable()
    {
        width = GetComponent<RectTransform>().sizeDelta.x;
        transform.localPosition = new Vector3(-width, 0f, 0f);
        _animationTime = 0f;
    }
    
    private void Update()
    {
        if (_animationTime >= animationDuration)
        {
            return;
        }

        _animationTime += Time.deltaTime;
        float t = _animationTime / animationDuration;
        float easedT = easeInOutCubic(t);

        transform.localPosition = new Vector3(-width * (1f - easedT), 0f, 0f);

        if (t >= 1f)
        {
            onShowCrowns.Raise(this);
            transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }

    private float easeInOutCubic(float x)
    {
        return x < 0.5f ? 4 * Mathf.Pow(x,3) : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
}
