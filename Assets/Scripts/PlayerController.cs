using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    public float m_WalkSpeed = 20f;
    public float m_MaxSpeed = 60f;
    public float m_DashSpeed = 1f;
    public float m_DashDelay = 1f;
    private float dashCooldown = 0f;
    private Vector3 moveInput = new Vector3(0f,0f,0f);
    private bool doDash = false;
    InputAction moveAction;
    InputAction dashAction;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        dashAction = InputSystem.actions.FindAction("Jump");
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveValue.x, 0.0f, moveValue.y).normalized;
        moveInput = move;

        if (dashCooldown > 0f)
        {
            dashCooldown -= Time.deltaTime;
        }
        if (dashAction.IsPressed() && dashCooldown <= 0 && moveInput.magnitude > 0f)
        {
            doDash = true;
            dashCooldown = m_DashDelay;
        } else
        {
            doDash = false;
        }
    }

    void FixedUpdate()
    {
        m_Rigidbody.AddForce(moveInput * m_WalkSpeed, ForceMode.Force);

        if (doDash)
        {
            m_Rigidbody.AddForce(moveInput * m_DashSpeed, ForceMode.Impulse);
        }

        if (m_Rigidbody.linearVelocity.magnitude > m_MaxSpeed) {
            m_Rigidbody.linearVelocity = m_Rigidbody.linearVelocity.normalized * m_MaxSpeed;
        }
    }
}
