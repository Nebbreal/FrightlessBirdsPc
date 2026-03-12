using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    InputAction moveAction;
    InputAction dashAction;
    InputAction attackAction;
    InputAction groundAction;
    [Header("Initialization Settings")]
    public Rigidbody m_Rigidbody;
    public Collider m_Collider;

    [Header("Player Settings")]
    public float m_StunTime = 1f;
    private float stunTimer = 0f;
    private bool isStunned = false;
    [Header("Walk Settings")]
    public float m_WalkSpeed = 20f;
    public float m_MaxSpeed = 60f;
    private Vector3 moveInput = new Vector3(0f,0f,0f);

    [Header("Dash Settings")]
    public float m_DashSpeed = 1f;
    public float m_DashDelay = 1f;
    private float dashCooldown = 0f;
    private bool doDash = false;
    
    [Header("Attack Settings")]
    public float m_AttackRange = 1f;
    public float m_AttackDelay = 1f;
    public LayerMask layerMask;
    private float attackCooldown = 0f;
    private bool doAttack = false;

    [Header("Ground Settings")]
    public float m_MaxGroundDuration = 1f;
    public float m_GroundDelay = 1f;
    public float m_GroundedMass = 10f;
    public float m_UnGroundedMass = 1f;
    public float m_GroundedDamping = 3f;
    public float m_UnGroundedDamping = 0.5f;
    private bool isGrounded = false;
    private float groundedTime = 0f;
    private float groundCooldown = 0f;

    private Vector3 lastMoveDirection = new Vector3(1f,0f,0f);

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        dashAction = InputSystem.actions.FindAction("Jump");
        attackAction = InputSystem.actions.FindAction("Attack");
        groundAction = InputSystem.actions.FindAction("Crouch");
    }

    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveValue.x, 0.0f, moveValue.y).normalized;
        moveInput = move;

#region Grounded Action
        isGrounded = false;
        if (groundCooldown > 0f)
        {
            groundCooldown -= Time.deltaTime;
        }

        if (groundAction.IsPressed() && groundCooldown <= 0)
        {
            groundedTime += Time.deltaTime;
            if (groundedTime >= m_MaxGroundDuration)
            {
                groundedTime = 0.0f;
                groundCooldown = m_GroundDelay;
            } else {
                isGrounded = true;
            }
        }
#endregion 

#region Dash Action
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
#endregion

#region Attack Action
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
        if (attackAction.IsPressed() && attackCooldown <= 0)
        {
            doAttack = true;
            attackCooldown = m_AttackDelay;
        } else
        {
            doAttack = false;
        }
#endregion

#region Is Stunned
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                isStunned = false;
                stunTimer = 0f;
            }

            moveInput = Vector3.zero;
            isGrounded = false;
            doAttack = false;
            doDash = false;
        }
#endregion

#region Is Grounded
        if (isGrounded)
        {
            moveInput = Vector3.zero;
            m_Rigidbody.mass = m_GroundedMass;
            m_Rigidbody.linearDamping = m_GroundedDamping;
            if (doAttack)
            {
                doAttack = false;
                attackCooldown = 0f;
            }
            if (doDash)
            {
                doDash = false;
                dashCooldown = 0f;
            }
        } else
        {
            m_Rigidbody.mass = m_UnGroundedMass;
            m_Rigidbody.linearDamping = m_UnGroundedDamping;
        }
#endregion

        if (m_Rigidbody.linearVelocity.magnitude > 0.1) {
            lastMoveDirection = m_Rigidbody.linearVelocity.normalized;
        }
    }

    void FixedUpdate()
    {
        m_Rigidbody.AddForce(moveInput * m_WalkSpeed, ForceMode.Force);

        if (doDash)
        {
            m_Rigidbody.AddForce(moveInput * m_DashSpeed, ForceMode.Impulse);
        }

        if (doAttack)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position + lastMoveDirection * (m_AttackRange + 0.2f), m_AttackRange, layerMask);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider == null || hitCollider == m_Collider)
                {
                    continue;
                }
                hitCollider.SendMessage("Stun");
            }
        }

        if (m_Rigidbody.linearVelocity.magnitude > m_MaxSpeed) {
            m_Rigidbody.linearVelocity = m_Rigidbody.linearVelocity.normalized * m_MaxSpeed;
        }
    }

    void OnDrawGizmos()
    {
        if (doAttack)
        {
            Gizmos.DrawSphere(transform.position + lastMoveDirection * (m_AttackRange + 0.2f), m_AttackRange);
        }

        if (isStunned)
        {
            Gizmos.color = new Color(0.75f, 0.0f, 0.0f, 0.75f);
            Gizmos.DrawSphere(transform.position + new Vector3(0f, 1f, 0f), 1.0f);
        }

        
        if (isGrounded)
        {
            Gizmos.color = new Color(0.0f, 0.75f, 0.0f, 0.75f);
            Gizmos.DrawSphere(transform.position + new Vector3(0f, 1f, 0f), 2.0f);
        }
    }

    public void Stun()
    {
        isStunned = true;
        stunTimer = m_StunTime;
    }
}
