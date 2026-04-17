using UnityEngine;

public class RoadRunnerCar : MonoBehaviour
{
    public Vector3 direction;
    public float resetLength;
    private Vector3 start;
    public float Strength = 50f;

    void Start()
    {
        start = transform.position;
    }

    void Update()
    {
        transform.position += direction;

        if ((transform.position - start).magnitude > resetLength)
        {
            transform.position = start;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.rigidbody) return;
        PlayerController p = collision.gameObject.GetComponent<PlayerController>();

        Rigidbody rb = collision.rigidbody;
        rb.linearVelocity = Vector3.zero;
        Vector3 dir = p.transform.position;
        dir.y = 0f;
        dir.Normalize();
        dir.y = 4f;
        dir *= Strength;
        rb.linearDamping = 0f;
        rb.AddForce(dir, ForceMode.Impulse);
        rb.useGravity = false;
        p.SetPlayerState(PlayerState.Stunned);
    }
}
