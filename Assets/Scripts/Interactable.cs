using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent<CollisionData> onAttackHit;
    public UnityEvent<CollisionData> onDashHit;
    public float knockbackStrength = 0.25f;
    public int player_id = -1;

    public void Break(CollisionData data)
    {
        PlayerController player = data.OriginCollider.GetComponent<PlayerController>();
        if (player.id == player_id)
        {
            gameObject.GetComponent<MeshCollider>().enabled = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void Knockback(CollisionData data)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(data.Direction * data.Speed+ Vector3.up * knockbackStrength, ForceMode.Impulse);


        PlayerController player = data.OriginCollider.GetComponent<PlayerController>();
        player.onDashHasHit.Invoke();
    }

    public void Reset()
    {
        gameObject.GetComponent<MeshCollider>().enabled = true;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
}
