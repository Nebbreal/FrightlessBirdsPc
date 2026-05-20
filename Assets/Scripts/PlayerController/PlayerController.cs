using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using EventSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Initialization Settings")] 
    public PlayerState playerState;
    public Rigidbody rb;
    public Collider col;
    public Transform attackHitbox;
    public Transform dashHitbox;
    public Transform braceHitbox;
    public Transform stunTransform;

    public UnityEvent<CollisionData> onAttackHit;
    public UnityEvent<CollisionData> onDashBracedHit;
    public UnityEvent<CollisionData> onDashHit;
    public UnityEvent onDashHasHit;

    

    [Header("Player Settings")] 
    public float stunTime = 1f;
    public float rotationSpeed = 0.15f;
    private float _stunTimer;

    [NonSerialized] public bool IsActive = false;
    [NonSerialized] public int ID = -1;

    [Header("Walk Settings")] 
    public float walkSpeed = 20f;
    public float maxSpeed = 60f;
    public float maxSpeedClampFactor = 0.8f;
    [NonSerialized] 
    public Vector3 _moveInput = new Vector3(0f, 0f, 0f);
    private Vector3 _moveThisFrame = new Vector3(0f, 0f, 0f);
    public Animator animator;

    [Header("Dash Settings")] 
    public float dashSpeed = 1f;
    public float dashDelay = 1f;
    [NonSerialized]
    public float _dashCooldown;
    [NonSerialized]
    public bool _doDash;
    public float dashDuration = 1f;
    public float dashEarlyEndSpeed = 10f;
    private float _dashTime;
    public float dashHitRange = 2f;
    public float dashKnockbackSpeed = 2f;
    private List<Collider> _dashCollided = new List<Collider>();

    [Header("Attack Settings")] 
    public float attackRange = 1f;

    public float attackRadius = 1f;
    public float attackDelay = 1f;
    public float attackDuration = 1f;
    public float attackKnockbackSpeed = 2f;
    public LayerMask layerMask;
    public LayerMask playerLayerMask;
    public LayerMask interactableLayerMask;
    [NonSerialized] 
    public float _attackCooldown;
    private float _attackTime;
    [NonSerialized] 
    public bool _doAttack;

    [Header("Braced Settings")] 
    public float bracedDelay = 1f;
    public float bracedReturnKnockbackSpeed = 2f;
    public float bracedReturnStunDuration = 0.3f;
    private bool _doBrace;
    private bool _holdingBrace;
    private float _bracedCooldown;

    [NonSerialized] 
    public Vector3 _lastMoveDirection = new Vector3(1f, 0f, 0f);

    [Header("Game events")]
    [SerializeField] 
    private GameEvent onSpawnDebugPlayers;
    public GameEvent onJoin;
    public GameEvent onLeave;
    public GameEvent onPlayerDash;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (playerState is PlayerState.InputRestricted)
        {
            _doAttack = false;
            _doBrace = false;
            _doDash = false;
            _moveThisFrame = Vector3.zero;
            return;
        }
        if (_holdingBrace && _bracedCooldown <= 0 && 
            playerState is PlayerState.Idle or PlayerState.Movement or PlayerState.Braced)
        {
            _doBrace = true;
        }

        _moveThisFrame = _moveInput;
        UpdateCooldowns();
        UpdateActiveTimer();

        UpdateStates();


        var facing = rb.linearVelocity;
        facing.y = 0f;
        if (facing.normalized.magnitude > 0.1)
        {
            _lastMoveDirection = Vector3.Slerp(_lastMoveDirection, facing.normalized, rotationSpeed).normalized;
            transform.rotation = Quaternion.LookRotation(_lastMoveDirection);
        }
    }

    private void KeepRbAwake()
    {
        if (rb.IsSleeping())
        {
            rb.WakeUp();
        }
    }

    public void Join(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (IsActive)
            {
                onLeave.Raise(this);
            }
            else
            {
                onJoin.Raise(this);
            }
        }
    }

    public void SpawnDebugPlayers(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        onSpawnDebugPlayers.Raise(this);
    }
    
    public void SetPlayerState(PlayerState newState)
    {
        ExitState(playerState, newState);
        switch (newState)
        {
            case PlayerState.Dash:
                _dashTime = dashDuration;
                dashHitbox.position = transform.position + _lastMoveDirection * (dashHitRange / 2f);
                // dashHitbox.localScale = new Vector3(dashHitRange * 2f, dashHitRange * 2f, dashHitRange * 2f);
                dashHitbox.gameObject.SetActive(true);
                _doBrace = false;
                _doAttack = false;
                onPlayerDash.Raise(this);
                break;
            case PlayerState.Attack:
                _attackTime = attackDuration;
                _doBrace = false;
                _doDash = false;
                break;
            case PlayerState.Stunned:
                _stunTimer = stunTime;
                stunTransform.gameObject.SetActive(true);
                _doAttack = false;
                _doDash = false;
                _doBrace = false;
                break;
            case PlayerState.Braced:
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                                 RigidbodyConstraints.FreezeRotationZ;
                braceHitbox.position = transform.position;
                braceHitbox.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                braceHitbox.gameObject.SetActive(true);
                _doAttack = false;
                _doDash = false;
                break;
            case PlayerState.InputRestricted:
                rb.constraints = RigidbodyConstraints.FreezeAll;
                _doAttack = false;
                _doDash = false;
                _doBrace = false;
                break;
        }

        playerState = newState;
    }

    private void ExitState(PlayerState current, PlayerState newState)
    {
        attackHitbox.gameObject.SetActive(false);
        dashHitbox.gameObject.SetActive(false);
        braceHitbox.gameObject.SetActive(false);
        stunTransform.gameObject.SetActive(false);
        switch (current)
        {
            case PlayerState.Movement:
                if (newState != PlayerState.Dash)
                {
                    animator.SetBool("IsWalking", false);
                }
                break;
            case PlayerState.Dash:
                if (newState != PlayerState.Movement)
                {
                    animator.SetBool("IsWalking", false);
                }
                
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

    private void UpdateStates()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                if (_moveInput.magnitude > 0f)
                {
                    SetPlayerState(PlayerState.Movement);
                    animator.SetBool("IsWalking", true);
                }

                IdleToState();
                break;
            case PlayerState.Movement:
                if (_moveInput.magnitude <= 0f)
                {
                    SetPlayerState(PlayerState.Idle);
                    animator.SetBool("IsWalking", false);
                }

                IdleToState();
                break;
            case PlayerState.Dash:
                CheckIsDashing();
                break;
            case PlayerState.Braced:
                if (!_doBrace)
                {
                    SetPlayerState(PlayerState.Idle);
                }
                CheckIsBraced();
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
    private void IdleToState()
    {
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

    private void UpdateCooldowns()
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

    private void UpdateActiveTimer()
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

    public void InputMove(InputAction.CallbackContext context)
    {
        Vector2 moveValue = context.action.ReadValue<Vector2>();
        
        switch (ID)
        {
            case 0: case 1:
                moveValue = new Vector2(moveValue.x, moveValue.y);
                break;
            case 2: case 3:
                moveValue = new Vector2(-moveValue.x, -moveValue.y);
                break;
        }
        Vector3 move = new Vector3(moveValue.x, 0.0f, moveValue.y).normalized;
        _moveInput = move;
    }

    public void InputBraced(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _holdingBrace = true;
        }

        if (context.canceled)
        {
            _holdingBrace = false;
        }
    }

    public void InputDash(InputAction.CallbackContext context)
    {
        if (context.action.IsPressed() && _dashCooldown <= 0 &&
            playerState is PlayerState.Idle or PlayerState.Movement)
        {
            _doDash = true;
        }
    }

    public void InputAttack(InputAction.CallbackContext context)
    {
        if (context.action.IsPressed() && _attackCooldown <= 0 && 
            playerState is PlayerState.Idle or PlayerState.Movement)
        {
            _doAttack = true;
        }
    }

    private void CheckIsDashing()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + _lastMoveDirection * (dashHitRange / 2f),
            dashHitRange, layerMask);
        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider || hitCollider == col || _dashCollided.Contains(hitCollider))
            {
                continue;
            }

            if ((playerLayerMask & (1 << hitCollider.gameObject.layer)) != 0)
            {
                _dashCollided.Add(hitCollider);
                hitCollider.GetComponent<PlayerController>().onDashHit.Invoke(
                    new CollisionData(rb.linearVelocity.normalized, dashKnockbackSpeed, col)
                );
            }
            else if ((interactableLayerMask & (1 << hitCollider.gameObject.layer)) != 0)
            {
                hitCollider.GetComponent<Interactable>().onDashHit.Invoke(
                    new CollisionData(_lastMoveDirection, attackKnockbackSpeed, col)
                );
            }
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

    private void CheckIsAttacking()
    {
        if (_attackTime <= 0)
        {
            SetPlayerState(PlayerState.Idle);
            _attackTime = 0f;
        }

        _moveThisFrame = Vector3.zero;
    }

    private void CheckIsStunned()
    {
        if (_stunTimer <= 0)
        {
            SetPlayerState(PlayerState.Idle);
            _stunTimer = 0f;
        }

        _moveThisFrame = Vector3.zero;
    }

    private void CheckIsBraced()
    {
        _moveThisFrame = Vector3.zero;
    }

    private void FixedUpdate()
    {
        KeepRbAwake();
        
        rb.AddForce(_moveThisFrame * walkSpeed, ForceMode.Force);

        if (_doDash)
        {
            ExecuteDash();
        }

        if (_doAttack)
        {
            ExecuteAttack();
        }

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, rb.linearVelocity.normalized * maxSpeed, maxSpeedClampFactor);
        }

        _doAttack = false;
        _doDash = false;
        _doBrace = false;
    }

    private void ExecuteAttack()
    {
        var dir = _lastMoveDirection;
        if (_moveInput.magnitude > 0f)
        {
            dir = _moveInput;
        }

        rb.linearVelocity = dir * rb.linearVelocity.magnitude;
        _lastMoveDirection = dir;
        transform.rotation = Quaternion.LookRotation(_lastMoveDirection);

        attackHitbox.position = transform.position + _lastMoveDirection * attackRange;
        attackHitbox.localScale = new Vector3(attackRadius, attackRadius, attackRadius) * 2f;
        attackHitbox.gameObject.SetActive(true);
        Collider[] hitColliders =
            Physics.OverlapSphere(transform.position + _lastMoveDirection * attackRange, attackRadius,
                layerMask);
        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider || hitCollider == col)
            {
                continue;
            }

            if ((playerLayerMask & (1 << hitCollider.gameObject.layer)) != 0)
            {
                hitCollider.GetComponent<PlayerController>().onAttackHit.Invoke(
                    new CollisionData(_lastMoveDirection, attackKnockbackSpeed, col)
                );
            }
            else if ((interactableLayerMask & (1 << hitCollider.gameObject.layer)) != 0)
            {
                hitCollider.GetComponent<Interactable>().onAttackHit.Invoke(
                    new CollisionData(_lastMoveDirection, attackKnockbackSpeed, col)
                );
            }
        }
    }

    private void ExecuteDash()
    {
        var dir = _lastMoveDirection;
        if (_moveThisFrame.magnitude > 0f)
        {
            dir = _moveThisFrame;
        }

        rb.linearVelocity = Vector3.zero;
        _lastMoveDirection = dir;
        rb.AddForce(dir * dashSpeed, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
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

    public void AttackStun(CollisionData data)
    {
        if (playerState != PlayerState.Dash)
        {
            Stun(data, stunTime);
        }
    }

    public void BracedReturnStun(CollisionData data)
    {
        Stun(data, bracedReturnStunDuration);
    }

    public void Stun(CollisionData data, float duration)
    {
        SetPlayerState(PlayerState.Stunned);
        _stunTimer = duration;
        Knockback(data);
    }

    public void HasHitWithDash()
    {
        rb.linearVelocity = Vector3.zero;
        _doDash = false;
        SetPlayerState(PlayerState.Idle);
    }

    public void DashKnockback(CollisionData data)
    {
        PlayerController player = data.OriginCollider.GetComponent<PlayerController>();
        if (playerState != PlayerState.Braced)
        {
            Knockback(data);
            player.onDashHasHit.Invoke();
        }
        else if (data.OriginCollider != null)
        {
            player.onDashBracedHit.Invoke(
                new CollisionData(-data.Direction, bracedReturnKnockbackSpeed, col)
            );
        }
    }

    public void Knockback(CollisionData data)
    {
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(data.Direction * data.Speed, ForceMode.Impulse);
    }
}
