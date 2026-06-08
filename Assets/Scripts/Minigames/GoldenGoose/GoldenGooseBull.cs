using UnityEngine;
using EventSystem;

public class GoldenGooseBull : MonoBehaviour
{
    public float speed;
    public float resetLength;
    private Vector3 start;
    public float Strength = 50f;
    public float UpStrengthMultiplier = 4f;

    private Rigidbody rb;
    public GameEvent onAttacked;

    private void Start()
    {
        start = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var direction = transform.forward * speed;
        rb.linearVelocity = direction;

        if ((transform.position - start).magnitude > resetLength)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!collision.attachedRigidbody) return;
        PlayerController p = collision.gameObject.GetComponent<PlayerController>();
        if (!p) return;
        onAttacked.Raise(p);

        Rigidbody rb2 = collision.attachedRigidbody;
        rb2.linearVelocity = Vector3.zero;
        Vector3 dir = p.transform.position;
        dir.y = 0f;
        dir.Normalize();
        dir.y = UpStrengthMultiplier;
        dir *= Strength;
        rb2.linearDamping = 0f;
        rb2.AddForce(dir, ForceMode.Impulse);
        rb2.useGravity = false;
        p.SetPlayerState(PlayerState.Stunned);
    }
}
