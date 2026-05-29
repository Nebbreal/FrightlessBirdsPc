using System;
using UnityEngine;

public class InteractableBlockTrigger : MonoBehaviour
{
    private PlayerController _playerController;
    
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
        if (!interactableRigidbody || interactableRigidbody.linearVelocity.magnitude < _playerController.requiredInteractableVelocity) return;
        
        float knockbackStrength = interactable.shieldKnockbackStrength;
        Vector3 direction = (other.transform.position - transform.position).normalized;
        
        interactable.Knockback(direction, knockbackStrength);
        _playerController.onInteractableBlock.Invoke();
    }
}
