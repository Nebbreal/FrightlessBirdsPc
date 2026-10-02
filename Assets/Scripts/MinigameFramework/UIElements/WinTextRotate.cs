using System;
using UnityEngine;
using UnityEngine.Assertions;

public class WinTextRotate : MonoBehaviour
{
    public float animationDuration;
    [Range (0f, 180f)]
    public float angle = 180f;
    private float _animationTime = 0f;
    private bool _isReversed;

    
    private void Update()
    {
        _animationTime += Time.deltaTime;
        
        if (_animationTime >= animationDuration)
        {
            _animationTime = -animationDuration;
        }
        
        var t = _animationTime / animationDuration;
        
        transform.rotation = Quaternion.AngleAxis(angle * Ease.EaseOutSine(t), Vector3.forward);
    }
}
