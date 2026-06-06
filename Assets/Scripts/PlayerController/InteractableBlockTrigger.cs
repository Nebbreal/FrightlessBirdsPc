using System;
using UnityEngine;

public class InteractableBlockTrigger : MonoBehaviour
{
    private PlayerController _playerController;
    private float _minimumVelocityThreshold = 0.5f;
    
    private void Start()
    {
        _playerController = gameObject.GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        
        if (!interactable) return;
        if (!interactable.isPushable) return;
        Rigidbody interactableRigidbody = interactable.GetComponent<Rigidbody>();
        if (!interactableRigidbody || interactableRigidbody.linearVelocity.magnitude < _minimumVelocityThreshold) return;
        
        if(Vector3.Dot(interactableRigidbody.linearVelocity.normalized, (transform.position - other.transform.position).normalized) <= 0) return;

        bool breaksShield = interactableRigidbody.linearVelocity.magnitude >
                            _playerController.requiredInteractableBreakVelocity;

        float knockbackStrength = breaksShield ? interactable.shieldBreakKnockbackStrength : interactable.shieldKnockbackStrength;
        
        Vector3 direction = (other.transform.position - transform.position).normalized;
        interactableRigidbody.linearVelocity = Vector3.zero;
        interactable.Knockback(direction, knockbackStrength);

        if (breaksShield)
        {
            _playerController.onInteractableBlock.Invoke();
        }
    }
}
