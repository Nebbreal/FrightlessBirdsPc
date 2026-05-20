using UnityEngine;

public class WinTextRotate : MonoBehaviour
{
    public float animationDuration;
    private float _animationTime = 0f;

    private void Update()
    {
        _animationTime += Time.deltaTime;
        if (_animationTime >= animationDuration)
        {
            _animationTime = -animationDuration;
        }
        var t = _animationTime / animationDuration;

        transform.rotation = Quaternion.AngleAxis(180f * Ease.InOutCubicNegInverted(t), Vector3.forward);
    }
}
