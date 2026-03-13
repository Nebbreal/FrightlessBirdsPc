using System.Collections.Generic;
using System.Data.Common;
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
    public PlayerState PState;
    public string MoveActionName = "Move";
    public string DashActionName = "Jump";
    public string AttackActionName = "Attack";
    public string GroundActionName = "Crouch";
    public Rigidbody Rb;
    public Collider Col;
    public ParticleSystem AttackParticles;

    public PlayerState TargetState = PlayerState.Idle;

    [Header("Player Settings")]
    public float StunTime = 1f;
    private float _stunTimer = 0f;
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
    public float DashHitRange = 2f;
    public float DashKnockbackSpeed = 2f;
    private List<Collider> _dashCollided = new List<Collider>();
    
    [Header("Attack Settings")]
    public float AttackRange = 1f;
    public float AttackDelay = 1f;
    public float AttackDuration = 1f;
    public float AttackKnockbackSpeed = 2f;
    public LayerMask Lm;
    private float _attackCooldown = 0f;
    private float _attackTime = 0f;
    private bool _doAttack = false;

    [Header("Braced Settings")]
    public float BracedDelay = 1f;
    public float BracedReturnKnockbackSpeed = 2f;
    private bool _doBrace = false;
    private float _bracedCooldown = 0f;

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

        if (PState != PlayerState.Stunned)
        {
            ActivateGrounded();
            ActivateDash();
            ActivateAttack();
        }

        UpdateCooldowns();
        UpdateActiveTimer();

        UpdateStates();
        
        if (Rb.linearVelocity.magnitude > 0.1) {
            _lastMoveDirection = Rb.linearVelocity.normalized;
            transform.rotation = Quaternion.LookRotation (_lastMoveDirection);
        }
    }

    void SetPlayerState(PlayerState newState)
    {
        switch (newState) {
            case PlayerState.Dash:
                _dashTime = DashDuration;
                break;
            case PlayerState.Attack:
                _attackTime = AttackDuration;
                break;
            case PlayerState.Stunned:
                _stunTimer = StunTime;
                break;
            case PlayerState.Braced:
                Rb.constraints = RigidbodyConstraints.FreezeAll;
                break;
        }
        ExitState(PState);
        PState = newState;
    }

    void ExitState(PlayerState current)
    {
        switch (current) {
            case PlayerState.Dash:
                _dashCooldown = DashDelay;
                _dashTime = DashDuration;
                _dashCollided.Clear();
                break;
            case PlayerState.Attack:
                _attackCooldown = AttackDelay;
                break;
            case PlayerState.Braced:
                Rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                _bracedCooldown = BracedDelay;
                break;
        }
    }

    void UpdateStates()
    {
        switch (PState)
        {
            case PlayerState.Idle:
                if (moveInput.magnitude > 0f)
                {
                    SetPlayerState(PlayerState.Movement);
                }
                IdleToState();
                break;
            case PlayerState.Movement:
                IdleToState();
                break;
            case PlayerState.Dash:
                CheckIsDashing();
                break;
            case PlayerState.Braced:
                if (!_doBrace && TargetState != PlayerState.Braced)
                {
                    SetPlayerState(PlayerState.Idle);
                }
                CheckIsGrounded();
                break;
            case PlayerState.Attack:
                CheckIsAttacking();
                break;
            case PlayerState.Stunned:
                CheckIsStunned();
                break;
        }
    }

    void IdleToState()
    {
        if (TargetState != PlayerState.Idle)
        {
            SetPlayerState(TargetState);
        }
        if (_doBrace)
        {
            SetPlayerState(PlayerState.Braced);
        } else if (_doDash)
        {
            SetPlayerState(PlayerState.Dash);
        } else if (_doAttack)
        {
            SetPlayerState(PlayerState.Attack);
        }
    }

    void UpdateCooldowns()
    {
        if (_dashCooldown > 0f)
        {
            _dashCooldown -= Time.deltaTime;
        }
        if (_bracedCooldown > 0f)
        {
            _bracedCooldown -= Time.deltaTime;
        }
        if (_attackCooldown > 0f)
        {
            _attackCooldown -= Time.deltaTime;
        }
    }

    void UpdateActiveTimer()
    {
        switch (PState)
        {
            case PlayerState.Dash:
                _dashTime -= Time.deltaTime;
                break;
            case PlayerState.Attack:
                _attackTime -= Time.deltaTime;
                break;
            case PlayerState.Stunned:
                _stunTimer -= Time.deltaTime;
                break;
        }
    }

    void ActivateGrounded()
    {
        if (_groundAction.IsPressed() && _bracedCooldown <= 0)
        {
            _doBrace = true;
        }
    }
    void ActivateDash()
    {
        if (_dashAction.IsPressed() && _dashCooldown <= 0 && moveInput.magnitude > 0f && PState != PlayerState.Dash)
        {
            _doDash = true;
        }

    }

    void ActivateAttack()
    {
        if (_attackAction.IsPressed() && _attackCooldown <= 0 && PState != PlayerState.Attack)
        {
            _doAttack = true;
        }
    }
    void CheckIsDashing()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + _lastMoveDirection * (DashHitRange / 2f), DashHitRange, Lm);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider == null || hitCollider == Col || _dashCollided.Contains(hitCollider))
            {
                continue;
            }
            _dashCollided.Add(hitCollider);
            hitCollider.SendMessage("DashKnockback", new CollisionData(Rb.linearVelocity.normalized, DashKnockbackSpeed, Col));
        }

        if (_dashTime <= 0.0f) {
            SetPlayerState(PlayerState.Movement);
        }
        if (_dashTime <= DashDuration - 0.1f && Rb.linearVelocity.magnitude < DashEarlyEndSpeed) {
            SetPlayerState(PlayerState.Idle);
        }
        
    }
    void CheckIsAttacking()
    {
        if (_attackTime <= 0)
        {
            SetPlayerState(PlayerState.Idle);
            _attackTime = 0f;
        }

        moveInput = Vector3.zero;
    }
    void CheckIsStunned()
    {
        if (_stunTimer <= 0)
        {
            SetPlayerState(PlayerState.Idle);
            _stunTimer = 0f;
        }

        moveInput = Vector3.zero;
    }
    void CheckIsGrounded()
    {
        if (PState == PlayerState.Braced)
        {
            moveInput = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        Rb.AddForce(moveInput * WalkSpeed, ForceMode.Force);

        if (_doDash)
        {
            Rb.AddForce(moveInput * DashSpeed, ForceMode.Impulse);
        }

        if (_doAttack)
        {
            var main = AttackParticles.main;
            main.startRotation = (Quaternion.LookRotation (_lastMoveDirection).y * Mathf.Rad2Deg) / 2f - 90f;
            AttackParticles.Play();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position + _lastMoveDirection * (AttackRange + 0.2f), AttackRange, Lm);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider == null || hitCollider == Col)
                {
                    continue;
                }
                hitCollider.SendMessage("Stun", new CollisionData(_lastMoveDirection, AttackKnockbackSpeed, Col));
            }
        }

        if (Rb.linearVelocity.magnitude > MaxSpeed) {
            Rb.linearVelocity = Rb.linearVelocity.normalized * MaxSpeed;
        }

        _doAttack = false;
        _doDash = false;
        _doBrace = false;
    }

    void OnDrawGizmos()
    {
        if (PState == PlayerState.Attack)
        {
            Gizmos.DrawSphere(transform.position + _lastMoveDirection * (AttackRange + 0.2f), AttackRange);
        }

        if (PState == PlayerState.Dash)
        {
            Gizmos.DrawSphere(transform.position + _lastMoveDirection * (DashHitRange / 2f), DashHitRange);
        }

        if (PState == PlayerState.Stunned)
        {
            Gizmos.color = new Color(0.75f, 0.0f, 0.0f, 0.75f);
            Gizmos.DrawSphere(transform.position + new Vector3(0f, 1f, 0f), 1.0f);
        }

        
        if (PState == PlayerState.Braced)
        {
            Gizmos.color = new Color(0.0f, 0.75f, 0.0f, 0.75f);
            Gizmos.DrawSphere(transform.position + new Vector3(0f, 1f, 0f), 2.0f);
        }
    }

    public void Stun(CollisionData data)
    {
        if (PState != PlayerState.Dash) {
            SetPlayerState(PlayerState.Stunned);
            Knockback(data);
        }
    }

    public void DashKnockback(CollisionData data)
    {
        if (PState != PlayerState.Braced)
        {
            Knockback(data);
        } else if (data.OriginCollider != null)
        {
            data.OriginCollider.SendMessage("Knockback", new CollisionData(-data.Direction, BracedReturnKnockbackSpeed, Col));
        }
    }

    public void Knockback(CollisionData data)
    {
        Rb.AddForce(data.Direction * data.Speed, ForceMode.Impulse);
    }
}
