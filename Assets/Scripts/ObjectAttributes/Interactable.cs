using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Interactable : MonoBehaviour
{
    [Header("Config")]
    public bool isBreakable = false;
    public bool isPushable = false;
    public float knockbackStrength = 0.25f;
    public float shieldKnockbackStrength = 2;
    public float shieldBreakKnockbackStrength = 5f;
    public float upStrength = 0.25f;
    public int playerID = -1;
    
    [Header("Events")]
    public UnityEvent<CollisionData> onAttackHit;
    public UnityEvent<CollisionData> onDashHit;

    public void Break(CollisionData data)
    {
        if (!isBreakable) return;
        
        PlayerController player = data.OriginCollider.GetComponent<PlayerController>();
        if (player.ID == playerID)
        {
            SetCollidersState(false);
        }
    }

    public void DashKnockback(CollisionData data)
    {
        if (!isPushable) return;
        
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(data.Direction * data.Speed * knockbackStrength + Vector3.up * upStrength, ForceMode.Impulse);

        PlayerController player = data.OriginCollider.GetComponent<PlayerController>();
        player.onDashHasHit.Invoke();
    }

    public void Knockback(Vector3 direction, float power)
    {
        if (!isPushable) return;
        
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(direction * power + Vector3.up * upStrength, ForceMode.Impulse);
    }

    public void Reset()
    {
        SetCollidersState(true);
    }
    
    private void SetCollidersState(bool isEnabled)
    {
        BoxCollider[] colliders = gameObject.GetComponents<BoxCollider>();
        foreach (BoxCollider boxCollider in colliders)
        {
            boxCollider.enabled = isEnabled;
        }

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = isEnabled;
        }
    }
}