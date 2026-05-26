using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using EventSystem;
using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine.Serialization;

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

    [FormerlySerializedAs("walkSpeed")] [Header("Walk Settings")] 
    public float acceleration = 20f;
    public float maxSpeed = 60f;
    public float maxSpeedClampFactor = 0.8f;
    public float defaultDecelerationMultiplier = 1.4f;
    [NonSerialized] 
    public Vector3 MoveInput = new Vector3(0f, 0f, 0f);
    private Vector3 _moveThisFrame = new Vector3(0f, 0f, 0f);
    public Animator animator;

    [Header("Dash Settings")] 
    public float dashSpeed = 1f;
    public float dashDelay = 1f;
    public float dashMoveInfluence = 0.7f;
    [NonSerialized]
    public float DashCooldown;
    [NonSerialized]
    public bool DoDash;
    public float dashDuration = 1f;
    public float dashEarlyEndSpeed = 10f;
    private float _dashTime;
    private Vector3 _dashDirection;
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
    public float AttackCooldown;
    private float _attackTime;
    [NonSerialized] 
    public bool DoAttack;

    [Header("Braced Settings")] 
    public float bracedDelay = 1f;
    public float bracedReturnKnockbackSpeed = 2f;
    public float bracedReturnStunDuration = 0.3f;
    [FormerlySerializedAs("playerDecelerationMultiplier")] public float braceDecelerationMultiplier = 0.9f;
    private bool _doBrace;
    private bool _holdingBrace;
    private float _bracedCooldown;

    [NonSerialized] 
    public Vector3 LastMoveDirection = new Vector3(1f, 0f, 0f);

    [Header("Game events")]
    [SerializeField] 
    private GameEvent onSpawnDebugPlayers;
    public GameEvent onJoin;
    public GameEvent onLeave;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (playerState is PlayerState.InputRestricted)
        {
            DoAttack = false;
            _doBrace = false;
            DoDash = false;
            _moveThisFrame = Vector3.zero;
            return;
        }
        if (_holdingBrace && _bracedCooldown <= 0 && 
            playerState is PlayerState.Idle or PlayerState.Movement or PlayerState.Braced)
        {
            _doBrace = true;
        }

        _moveThisFrame = MoveInput;
        UpdateCooldowns();
        UpdateActiveTimer();

        UpdateStates();

        var facing = rb.linearVelocity;
        facing.y = 0f;
        if (facing.normalized.magnitude > 0.1)
        {
            LastMoveDirection = Vector3.Slerp(LastMoveDirection, facing.normalized, rotationSpeed).normalized;
            transform.rotation = Quaternion.LookRotation(LastMoveDirection);
        }
    }
    
    private void FixedUpdate()
    {
        KeepRbAwake();

        if (playerState is not PlayerState.Dash)
        {
            rb.AddForce(_moveThisFrame * acceleration, ForceMode.Force);
        }
        else if (playerState is PlayerState.Dash)
        {
            Vector3 movementInfluence;

            float moveDotProduct = Vector3.Dot(_moveThisFrame, _dashDirection);
            
            //Straight ahead
            if (Mathf.Approximately(moveDotProduct, 1f))
            {
                movementInfluence = Vector3.zero;
            }
            else if(Mathf.Approximately(moveDotProduct, -1f)) //Opposite direction
            {
                movementInfluence = -_dashDirection;
            }
            else
            {
                movementInfluence = _moveThisFrame;
            }
            Debug.Log(rb.linearVelocity.magnitude);
            //Cut off movement boosting dash in the movement direction
            rb.AddForce(movementInfluence.normalized * (acceleration * dashMoveInfluence), ForceMode.Force);
        }

        if (DoDash)
        {
            ExecuteDash();
        }

        if (DoAttack)
        {
            ExecuteAttack();
        }

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, rb.linearVelocity.normalized * maxSpeed, maxSpeedClampFactor);
        }

        DoAttack = false;
        DoDash = false;
        _doBrace = false;
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
                _dashDirection = _moveThisFrame.normalized;
                dashHitbox.position = transform.position + LastMoveDirection * (dashHitRange / 2f);
                // dashHitbox.localScale = new Vector3(dashHitRange * 2f, dashHitRange * 2f, dashHitRange * 2f);
                dashHitbox.gameObject.SetActive(true);
                _doBrace = false;
                DoAttack = false;
                break;
            case PlayerState.Attack:
                _attackTime = attackDuration;
                _doBrace = false;
                DoDash = false;
                break;
            case PlayerState.Stunned:
                _stunTimer = stunTime;
                stunTransform.gameObject.SetActive(true);
                DoAttack = false;
                DoDash = false;
                _doBrace = false;
                break;
            case PlayerState.Braced:
                /*rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                                 RigidbodyConstraints.FreezeRotationZ;*/
                braceHitbox.position = transform.position;
                braceHitbox.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                braceHitbox.gameObject.SetActive(true);
                DoAttack = false;
                DoDash = false;
                break;
            case PlayerState.InputRestricted:
                rb.constraints = RigidbodyConstraints.FreezeAll;
                DoAttack = false;
                DoDash = false;
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
                
                DashCooldown = dashDelay;
                _dashTime = dashDuration;
                _dashCollided.Clear();
                break;
            case PlayerState.Attack:
                AttackCooldown = attackDelay;
                break;
            case PlayerState.Braced:
                /*rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                                 RigidbodyConstraints.FreezeRotationZ;
                _bracedCooldown = bracedDelay;*/
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
                if (MoveInput.magnitude > 0f)
                {
                    SetPlayerState(PlayerState.Movement);
                    animator.SetBool("IsWalking", true);
                }
                
                DampenMovement();
                
                IdleToState();
                break;
            case PlayerState.Movement:
                if (MoveInput.magnitude <= 0f)
                {
                    SetPlayerState(PlayerState.Idle);
                    animator.SetBool("IsWalking", false);
                }
                
                DampenMovement();

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
        else if (DoDash)
        {
            SetPlayerState(PlayerState.Dash);
        }
        else if (DoAttack)
        {
            SetPlayerState(PlayerState.Attack);
        }
    }

    private void UpdateCooldowns()
    {
        if (DashCooldown > 0f)
        {
            DashCooldown -= Time.deltaTime;
        }

        if (_bracedCooldown > 0f)
        {
            _bracedCooldown -= Time.deltaTime;
        }

        if (AttackCooldown > 0f)
        {
            AttackCooldown -= Time.deltaTime;
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
        
        MoveInput = move;
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
        if (context.action.IsPressed() && DashCooldown <= 0 &&
            playerState is PlayerState.Idle or PlayerState.Movement)
        {
            DoDash = true;
        }
    }

    public void InputAttack(InputAction.CallbackContext context)
    {
        if (context.action.IsPressed() && AttackCooldown <= 0 && 
            playerState is PlayerState.Idle or PlayerState.Movement)
        {
            DoAttack = true;
        }
    }
    
    private void CheckIsDashing()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + LastMoveDirection * (dashHitRange / 2f),
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
                    new CollisionData(LastMoveDirection, attackKnockbackSpeed, col)
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
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, braceDecelerationMultiplier * Time.fixedDeltaTime);
    }

    private void ExecuteAttack()
    {
        var dir = LastMoveDirection;
        if (MoveInput.magnitude > 0f)
        {
            dir = MoveInput;
        }

        rb.linearVelocity = dir * rb.linearVelocity.magnitude;
        LastMoveDirection = dir;
        transform.rotation = Quaternion.LookRotation(LastMoveDirection);

        attackHitbox.position = transform.position + LastMoveDirection * attackRange;
        attackHitbox.localScale = new Vector3(attackRadius, attackRadius, attackRadius) * 2f;
        attackHitbox.gameObject.SetActive(true);
        Collider[] hitColliders =
            Physics.OverlapSphere(transform.position + LastMoveDirection * attackRange, attackRadius,
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
                    new CollisionData(LastMoveDirection, attackKnockbackSpeed, col)
                );
            }
            else if ((interactableLayerMask & (1 << hitCollider.gameObject.layer)) != 0)
            {
                hitCollider.GetComponent<Interactable>().onAttackHit.Invoke(
                    new CollisionData(LastMoveDirection, attackKnockbackSpeed, col)
                );
            }
        }
    }

    private void ExecuteDash()
    {
        var dir = LastMoveDirection;
        if (_moveThisFrame.magnitude > 0f)
        {
            dir = _moveThisFrame;
        }

        rb.linearVelocity = Vector3.zero;
        LastMoveDirection = dir;
        rb.AddForce(dir * dashSpeed, ForceMode.Impulse);
    }
    
    private void DampenMovement()
    {
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, defaultDecelerationMultiplier * Time.fixedDeltaTime);
    }

    private void OnDrawGizmos()
    {
        if (playerState == PlayerState.Attack)
        {
            Gizmos.DrawSphere(transform.position + LastMoveDirection * (attackRange + 0.2f), attackRange);
        }

        if (playerState == PlayerState.Dash)
        {
            Gizmos.DrawSphere(transform.position + LastMoveDirection * (dashHitRange / 2f), dashHitRange);
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
        DoDash = false;
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
