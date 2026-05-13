using UnityEngine;

public class WinTextRotate : MonoBehaviour
{
    public float animationDuration;
    private float _animationTime = 1f;

    void Update()
    {
        _animationTime += Time.deltaTime;
        if (_animationTime >= animationDuration)
        {
            _animationTime = -animationDuration;
        }
        var t = _animationTime / animationDuration;

        transform.rotation = Quaternion.AngleAxis(180f * ease(t), Vector3.forward);
    }

    float ease(float t)
    {
        if (Mathf.Abs(t) < 0.5)
        {
            return Mathf.Sign(t) * 4f * Mathf.Pow(t, 3);
        }

        return 1 - (Mathf.Pow(-2*Mathf.Abs(t)+2, 3) / 2f);
    }
}
