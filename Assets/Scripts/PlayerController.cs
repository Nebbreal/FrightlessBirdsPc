using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputAction _moveAction;
    private InputAction _dashAction;
    private InputAction _attackAction;
    private InputAction _groundAction;
    [Header("Initialization Settings")]
    public string MoveActionName = "Move";
    public string DashActionName = "Jump";
    public string AttackActionName = "Attack";
    public string GroundActionName = "Crouch";
    public Rigidbody Rb;
    public Collider Col;

    [Header("Player Settings")]
    public float StunTime = 1f;
    private float _stunTimer = 0f;
    private bool _isStunned = false;
    [Header("Walk Settings")]
    public float WalkSpeed = 20f;
    public float MaxSpeed = 60f;
    private Vector3 moveInput = new Vector3(0f,0f,0f);

    [Header("Dash Settings")]
    public float DashSpeed = 1f;
    public float DashDelay = 1f;
    private float _dashCooldown = 0f;
    private bool _doDash = false;
    public float DashDuration = 1f;
    public float DashEarlyEndSpeed = 10f;
    private float _dashTime = 0f;
    private bool _isDashing = false;
    public float DashHitRange = 2f;
    public float DashKnockbackSpeed = 2f;
    private List<Collider> _dashCollided = new List<Collider>();
    
    [Header("Attack Settings")]
    public float AttackRange = 1f;
    public float AttackDelay = 1f;
    public LayerMask Lm;
    private float _attackCooldown = 0f;
    private bool _doAttack = false;

    [Header("Ground Settings")]
    public float MaxGroundDuration = 1f;
    public float GroundDelay = 1f;
    public float GroundedMass = 10f;
    public float UngroundedMass = 1f;
    public float GroundedDamping = 3f;
    public float UngroundedDamping = 0.5f;
    private bool _isGrounded = false;
    private float _groundedTime = 0f;
    private float _groundCooldown = 0f;

    private Vector3 _lastMoveDirection = new Vector3(1f,0f,0f);

    private void Start()
    {
        _moveAction = InputSystem.actions.FindAction(MoveActionName);
        _dashAction = InputSystem.actions.FindAction(DashActionName);
        _attackAction = InputSystem.actions.FindAction(AttackActionName);
        _groundAction = InputSystem.actions.FindAction(GroundActionName);
    }

    void Update()
    {
        Vector2 moveValue = _moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveValue.x, 0.0f, moveValue.y).normalized;
        moveInput = move;

        Ground();
        Dash();
        Attack();
        CheckIsStunned();
        CheckIsGrounded();
        CheckIsDashing();
        
        if (Rb.linearVelocity.magnitude > 0.1) {
            _lastMoveDirection = Rb.linearVelocity.normalized;
        }
    }

    void Ground()
    {
        _isGrounded = false;
        if (_groundCooldown > 0f)
        {
            _groundCooldown -= Time.deltaTime;
        }

        if (_groundAction.IsPressed() && _groundCooldown <= 0)
        {
            _groundedTime += Time.deltaTime;
            if (_groundedTime >= MaxGroundDuration)
            {
                _groundedTime = 0.0f;
                _groundCooldown = GroundDelay;
            } else {
                _isGrounded = true;
            }
        }
    }
    void Dash()
    {
        if (_dashCooldown > 0f)
        {
            _dashCooldown -= Time.deltaTime;
        }
        if (_dashAction.IsPressed() && _dashCooldown <= 0 && moveInput.magnitude > 0f && !_isDashing)
        {
            _doDash = true;
        } else
        {
            _doDash = false;
        }

    }

    void Attack()
    {
        if (_attackCooldown > 0f)
        {
            _attackCooldown -= Time.deltaTime;
        }
        if (_attackAction.IsPressed() && _attackCooldown <= 0)
        {
            _doAttack = true;
            _attackCooldown = AttackDelay;
        } else
        {
            _doAttack = false;
        }
    }
    void CheckIsDashing()
    {
        if (_isDashing)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, DashHitRange, Lm);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider == null || hitCollider == Col || _dashCollided.Contains(hitCollider))
                {
                    continue;
                }
                _dashCollided.Add(hitCollider);
                hitCollider.SendMessage("Knockback", Rb.linearVelocity.normalized * DashKnockbackSpeed);
            }

            if (_dashTime > 0.0f)
            {
                _dashTime -= Time.deltaTime;
            } else {
                _isDashing = false;
                _dashCooldown = DashDelay;
                _dashTime = DashDuration;
                _dashCollided.Clear();
            }
            if (Rb.linearVelocity.magnitude < DashEarlyEndSpeed) {
                _isDashing = false;
                _dashCooldown = DashDelay;
                _dashTime = DashDuration;
                _dashCollided.Clear();
            }
        }
    }
    void CheckIsStunned()
    {
        if (_isStunned)
        {
            _stunTimer -= Time.deltaTime;
            if (_stunTimer <= 0)
            {
                _isStunned = false;
                _stunTimer = 0f;
            }

            moveInput = Vector3.zero;
            _isGrounded = false;
            _doAttack = false;
            _doDash = false;
        }
    }
    void CheckIsGrounded()
    {
        if (_isGrounded)
        {
            moveInput = Vector3.zero;
            Rb.mass = GroundedMass;
            Rb.linearDamping = GroundedDamping;
            if (_doAttack)
            {
                _doAttack = false;
                _attackCooldown = 0f;
            }
            if (_doDash)
            {
                _doDash = false;
                _dashCooldown = 0f;
            }
        } else
        {
            Rb.mass = UngroundedMass;
            Rb.linearDamping = UngroundedDamping;
        }
    }

    void FixedUpdate()
    {
        Rb.AddForce(moveInput * WalkSpeed, ForceMode.Force);

        if (_doDash)
        {
            Rb.AddForce(moveInput * DashSpeed, ForceMode.Impulse);
            _isDashing = true;
        }

        if (_doAttack)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position + _lastMoveDirection * (AttackRange + 0.2f), AttackRange, Lm);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider == null || hitCollider == Col)
                {
                    continue;
                }
                hitCollider.SendMessage("Stun");
            }
        }

        if (Rb.linearVelocity.magnitude > MaxSpeed) {
            Rb.linearVelocity = Rb.linearVelocity.normalized * MaxSpeed;
        }
    }

    void OnDrawGizmos()
    {
        if (_doAttack)
        {
            Gizmos.DrawSphere(transform.position + _lastMoveDirection * (AttackRange + 0.2f), AttackRange);
        }

        if (_isStunned)
        {
            Gizmos.color = new Color(0.75f, 0.0f, 0.0f, 0.75f);
            Gizmos.DrawSphere(transform.position + new Vector3(0f, 1f, 0f), 1.0f);
        }

        
        if (_isGrounded)
        {
            Gizmos.color = new Color(0.0f, 0.75f, 0.0f, 0.75f);
            Gizmos.DrawSphere(transform.position + new Vector3(0f, 1f, 0f), 2.0f);
        }
    }

    public void Stun()
    {
        _isStunned = true;
        _stunTimer = StunTime;
    }

    public void Knockback(Vector3 direction)
    {
        Rb.AddForce(direction, ForceMode.Impulse);
    }
}
