using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputAction _moveAction;
    private InputAction _dashAction;
    private InputAction _attackAction;
    private InputAction _groundAction;
    [Header("Initialization Settings")] 
    public PlayerState playerState;
    public string moveActionName = "Move";
    public string dashActionName = "Jump";
    public string attackActionName = "Attack";
    public string groundActionName = "Crouch";
    public Rigidbody rb;
    public Collider col;
    public ParticleSystem attackParticles;

    public PlayerState targetState = PlayerState.Idle;

    [Header("Player Settings")] public float stunTime = 1f;
    private float _stunTimer = 0f;
    [Header("Walk Settings")] public float walkSpeed = 20f;
    public float maxSpeed = 60f;
    private Vector3 _moveInput = new Vector3(0f, 0f, 0f);

    [Header("Dash Settings")] public float dashSpeed = 1f;
    public float dashDelay = 1f;
    private float _dashCooldown = 0f;
    private bool _doDash = false;
    public float dashDuration = 1f;
    public float dashEarlyEndSpeed = 10f;
    private float _dashTime = 0f;
    public float dashHitRange = 2f;
    public float dashKnockbackSpeed = 2f;
    private List<Collider> _dashCollided = new List<Collider>();

    [Header("Attack Settings")] public float attackRange = 1f;
    public float attackDelay = 1f;
    public float attackDuration = 1f;
    public float attackKnockbackSpeed = 2f;
    public LayerMask layerMask;
    private float _attackCooldown = 0f;
    private float _attackTime = 0f;
    private bool _doAttack = false;

    [Header("Braced Settings")] public float bracedDelay = 1f;
    public float bracedReturnKnockbackSpeed = 2f;
    private bool _doBrace = false;
    private float _bracedCooldown = 0f;

    private Vector3 _lastMoveDirection = new Vector3(1f, 0f, 0f);

    private void Start()
    {
        _moveAction = InputSystem.actions.FindAction(moveActionName);
        _dashAction = InputSystem.actions.FindAction(dashActionName);
        _attackAction = InputSystem.actions.FindAction(attackActionName);
        _groundAction = InputSystem.actions.FindAction(groundActionName);
    }

    void Update()
    {
        Vector2 moveValue = _moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveValue.x, 0.0f, moveValue.y).normalized;
        _moveInput = move;

        if (playerState != PlayerState.Stunned)
        {
            InputBraced();
            InputDash();
            InputAttack();
        }

        UpdateCooldowns();
        UpdateActiveTimer();

        UpdateStates();

        if (rb.linearVelocity.magnitude > 0.1)
        {
            _lastMoveDirection = rb.linearVelocity.normalized;
            transform.rotation = Quaternion.LookRotation(_lastMoveDirection);
        }
    }

    void SetPlayerState(PlayerState newState)
    {
        switch (newState)
        {
            case PlayerState.Dash:
                _dashTime = dashDuration;
                break;
            case PlayerState.Attack:
                _attackTime = attackDuration;
                break;
            case PlayerState.Stunned:
                _stunTimer = stunTime;
                break;
            case PlayerState.Braced:
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                                 RigidbodyConstraints.FreezeRotationZ;
                break;
            case PlayerState.InputRestricted:
                rb.constraints = RigidbodyConstraints.FreezeAll;
                break;
        }

        ExitState(playerState);
        playerState = newState;
    }

    void ExitState(PlayerState current)
    {
        switch (current)
        {
            case PlayerState.Dash:
                _dashCooldown = dashDelay;
                _dashTime = dashDuration;
                _dashCollided.Clear();
                break;
            case PlayerState.Attack:
                _attackCooldown = attackDelay;
                break;
            case PlayerState.Braced:
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                                 RigidbodyConstraints.FreezeRotationZ;
                _bracedCooldown = bracedDelay;
                break;
            case PlayerState.InputRestricted:
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                                 RigidbodyConstraints.FreezeRotationZ;
                break;
        }
    }

    void UpdateStates()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                if (_moveInput.magnitude > 0f)
                {
                    SetPlayerState(PlayerState.Movement);
                }

                IdleToState();
                break;
            case PlayerState.Movement:
                if (_moveInput.magnitude <= 0f)
                {
                    SetPlayerState(PlayerState.Idle);
                }

                IdleToState();
                break;
            case PlayerState.Dash:
                CheckIsDashing();
                break;
            case PlayerState.Braced:
                if (!_doBrace && targetState != PlayerState.Braced)
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

    // Go from an Idle state (Idle/Movement) to actual state based on player inputs
    void IdleToState()
    {
        // Target state is the state that a bot tries to keep (Braced state in the example scene)
        if (targetState != PlayerState.Idle)
        {
            SetPlayerState(targetState);
        }

        if (_doBrace)
        {
            SetPlayerState(PlayerState.Braced);
        }
        else if (_doDash)
        {
            SetPlayerState(PlayerState.Dash);
        }
        else if (_doAttack)
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
        switch (playerState)
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

    void InputBraced()
    {
        if (_groundAction.IsPressed() && _bracedCooldown <= 0)
        {
            _doBrace = true;
        }
    }

    void InputDash()
    {
        if (_dashAction.IsPressed() && _dashCooldown <= 0 &&
            playerState != PlayerState.Dash)
        {
            _doDash = true;
        }
    }

    void InputAttack()
    {
        if (_attackAction.IsPressed() && _attackCooldown <= 0 && playerState != PlayerState.Attack)
        {
            _doAttack = true;
        }
    }

    void CheckIsDashing()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + _lastMoveDirection * (dashHitRange / 2f),
            dashHitRange, layerMask);
        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider || hitCollider == col || _dashCollided.Contains(hitCollider))
            {
                continue;
            }

            _dashCollided.Add(hitCollider);
            hitCollider.SendMessage("DashKnockback",
                new CollisionData(rb.linearVelocity.normalized, dashKnockbackSpeed, col));
        }

        if (_dashTime <= 0.0f)
        {
            SetPlayerState(PlayerState.Movement);
        }

        if (_dashTime <= dashDuration - 0.1f && rb.linearVelocity.magnitude < dashEarlyEndSpeed)
        {
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

        _moveInput = Vector3.zero;
    }

    void CheckIsStunned()
    {
        if (_stunTimer <= 0)
        {
            SetPlayerState(PlayerState.Idle);
            _stunTimer = 0f;
        }

        _moveInput = Vector3.zero;
    }

    void CheckIsGrounded()
    {
        if (playerState == PlayerState.Braced)
        {
            _moveInput = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(_moveInput * walkSpeed, ForceMode.Force);

        if (_doDash)
        {
            var dir = _lastMoveDirection;
            if (_moveInput.magnitude > 0f)
            {
                dir = _moveInput;
            }
            rb.AddForce(dir * dashSpeed, ForceMode.Impulse);
        }

        if (_doAttack)
        {
            var main = attackParticles.main;
            main.startRotation = (Quaternion.LookRotation(_lastMoveDirection).y * Mathf.Rad2Deg) / 2f - 90f;
            attackParticles.Play();
            Collider[] hitColliders =
                Physics.OverlapSphere(transform.position + _lastMoveDirection * (attackRange + 0.2f), attackRange,
                    layerMask);
            foreach (var hitCollider in hitColliders)
            {
                if (!hitCollider || hitCollider == col)
                {
                    continue;
                }

                hitCollider.SendMessage("Stun", new CollisionData(_lastMoveDirection, attackKnockbackSpeed, col));
            }
        }

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        _doAttack = false;
        _doDash = false;
        _doBrace = false;
    }

    void OnDrawGizmos()
    {
        if (playerState == PlayerState.Attack)
        {
            Gizmos.DrawSphere(transform.position + _lastMoveDirection * (attackRange + 0.2f), attackRange);
        }

        if (playerState == PlayerState.Dash)
        {
            Gizmos.DrawSphere(transform.position + _lastMoveDirection * (dashHitRange / 2f), dashHitRange);
        }

        if (playerState == PlayerState.Stunned)
        {
            Gizmos.color = new Color(0.75f, 0.0f, 0.0f, 0.75f);
            Gizmos.DrawSphere(transform.position + new Vector3(0f, 1f, 0f), 1.0f);
        }


        if (playerState == PlayerState.Braced)
        {
            Gizmos.color = new Color(0.0f, 0.75f, 0.0f, 0.75f);
            Gizmos.DrawSphere(transform.position + new Vector3(0f, 1f, 0f), 2.0f);
        }
    }

    public void Stun(CollisionData data)
    {
        if (playerState != PlayerState.Dash)
        {
            SetPlayerState(PlayerState.Stunned);
            Knockback(data);
        }
    }

    public void DashKnockback(CollisionData data)
    {
        if (playerState != PlayerState.Braced)
        {
            Knockback(data);
        }
        else if (data.OriginCollider != null)
        {
            data.OriginCollider.SendMessage("Knockback",
                new CollisionData(-data.Direction, bracedReturnKnockbackSpeed, col));
        }
    }

    public void Knockback(CollisionData data)
    {
        rb.AddForce(data.Direction * data.Speed, ForceMode.Impulse);
    }
}
