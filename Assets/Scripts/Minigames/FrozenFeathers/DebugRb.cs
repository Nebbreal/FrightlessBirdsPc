using System;
using UnityEngine;

public class DebugRb : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        _rigidbody.WakeUp();
    }
}
