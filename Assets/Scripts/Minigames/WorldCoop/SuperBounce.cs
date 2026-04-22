using UnityEngine;

public class SuperBounce : MonoBehaviour
{
    [SerializeField] private float bounceMultiplier = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null)
        {
            Vector3 bounceDir = (collision.transform.position - transform.position);

            bounceDir.y = 0; 
            bounceDir = bounceDir.normalized;
            // bounceDir.y = 0.1f; // Optional: Add a slight upward force to make the bounce more dynamic

            rb.linearVelocity = Vector3.zero; 

            rb.AddForce(bounceDir * bounceMultiplier, ForceMode.Impulse);
        }
    }
}
