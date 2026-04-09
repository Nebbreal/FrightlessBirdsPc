using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent<CollisionData> onAttackHit;

    public void Break(CollisionData data)
    {
        gameObject.GetComponent<MeshCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        PlayerController player = data.OriginCollider.GetComponent<PlayerController>();
        player.onDashHasHit.Invoke();
    }

    public void Knockback(CollisionData data)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(data.Direction * data.Speed+ Vector3.up * 0.25f, ForceMode.Impulse);


        PlayerController player = data.OriginCollider.GetComponent<PlayerController>();
        player.onDashHasHit.Invoke();
    }

    public void Reset()
    {
        gameObject.GetComponent<MeshCollider>().enabled = true;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
}
