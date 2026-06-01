using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using EventSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    private static readonly int IsAnimatingWalk = Animator.StringToHash("IsWalking");
    private static readonly int IsAnimatingBlock = Animator.StringToHash("IsBlocking");

    private Collider[] _hitCollidersBuffer;
    
    [Header("Initialization Settings")] 
    public PlayerState playerState;
    public Rigidbody rb;
    public Collider col;
    public Transform attackHitbox;
    public Transform dashHitbox;
    public Transform braceHitbox;
    public Transform stunTransform;
    [Range(0f, 25f)]
    [Tooltip("How many hit players the PlayerController can detect at once")]
    public int hitCollidersBufferSize = 8;

    public UnityEvent<CollisionData> onAttackHit;
    public UnityEvent<CollisionData> onDashBlockHit;
    public UnityEvent<CollisionData> onDashHit;
    public UnityEvent onInteractableBlock;
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
    [FormerlySerializedAs("animator")] public Animator playerAnimator;

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

    [Header("Block Settings")] 
    public float maxBlockCharge = 3f;
    private float _blockCharge;
    public float blockReturnKnockbackSpeed = 2f;
    public float blockReturnStunDuration = 0.3f;
    public float blockDecelerationMultiplier = 0.9f;
    private bool _doBlock;
    private bool _isHoldingBlock;
    private bool _canBlock = true;
    public float requiredInteractableVelocity = 8f;
    [SerializeField]
    private GameObject interactableBlockTrigger;
    //Not sure how to handle putting this in CharacterSoundController
    [SerializeField] 
    private AudioClip shieldBreakAudioClip;
    private Animator _blockAnimator;

    [NonSerialized] 
    public Vector3 LastMoveDirection = new Vector3(1f, 0f, 0f);

    [Header("Game events")]
    [SerializeField] 
    private GameEvent onSpawnDebugPlayers;
    [SerializeField]
    public GameEvent onJoin;
    [SerializeField]
    public GameEvent onLeave;
    [SerializeField]
    private GameEvent onPlaySoundClipOnce;

    private void Start()
    {
        playerAnimator = GetComponentInChildren<PlayerModel>().GetComponent<Animator>();
        _blockAnimator = GetComponentInChildren<EggShield>().GetComponent<Animator>();
        _hitCollidersBuffer = new Collider[hitCollidersBufferSize];
        _blockCharge = maxBlockCharge;
    }

    private void Update()
    {
        if (playerState is PlayerState.InputRestricted)
        {
            DoAttack = false;
            _doBlock = false;
            DoDash = false;
            _moveThisFrame = Vector3.zero;
            return;
        }
        if (_isHoldingBlock && _canBlock && 
            playerState is PlayerState.Idle or PlayerState.Movement or PlayerState.Blocking)
        {
            _doBlock = true;
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
            CheckDashMovementInfluence();
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
        
        //Manual override due to weird interaction with input actions and the state machine
        DoAttack = false;
        DoDash = false;
        _doBlock = false;
    }

    private void CheckDashMovementInfluence()
    {
        //Check if player is moving towards dash direction and remove the forward force if it is the case
        Vector3 tangentDashDirection = Vector3.Cross(rb.linearVelocity.normalized, Vector3.up);
        float movementInfluence = Vector3.Dot(_moveThisFrame, tangentDashDirection);
            
        Vector3 newMoveDir = movementInfluence * tangentDashDirection;
        if (newMoveDir.sqrMagnitude > 0f)
        {
            rb.AddForce(newMoveDir * (acceleration * dashMoveInfluence), ForceMode.Force);
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
        EnterPlayerState(newState);
    }

    private void EnterPlayerState(PlayerState newState)
    {
        switch (newState)
        {
            case PlayerState.Movement:
                if (!playerAnimator.GetBool(IsAnimatingWalk))
                {
                    playerAnimator.SetBool(IsAnimatingWalk, true);
                }
                break;
            case PlayerState.Dash:
                if (!playerAnimator.GetBool(IsAnimatingWalk))
                {
                    playerAnimator.SetBool(IsAnimatingWalk, true);
                }
                
                _dashTime = dashDuration;
                dashHitbox.position = transform.position + LastMoveDirection * (dashHitRange / 2f);
                dashHitbox.gameObject.SetActive(true);
                _doBlock = false;
                DoAttack = false;
                break;
            case PlayerState.Attack:
                _attackTime = attackDuration;
                _doBlock = false;
                DoDash = false;
                break;
            case PlayerState.Stunned:
                _stunTimer = stunTime;
                stunTransform.gameObject.SetActive(true);
                DoAttack = false;
                DoDash = false;
                _doBlock = false;
                break;
            case PlayerState.Blocking:
                if (_canBlock && _blockCharge >= maxBlockCharge * 0.5)
                {
                    playerAnimator.SetBool(IsAnimatingBlock, true);
                    _blockAnimator.SetBool(IsAnimatingBlock, true);
                    
                    braceHitbox.position = transform.position;
                    braceHitbox.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    _blockAnimator.Play("Block", 0, 1f);
                    interactableBlockTrigger.SetActive(true);
                    braceHitbox.gameObject.SetActive(true);
                    DoAttack = false;
                    DoDash = false;
                }
                else
                {
                    SetPlayerState(PlayerState.Idle);
                    return;
                }
                
                break;
            case PlayerState.InputRestricted:
                rb.constraints = RigidbodyConstraints.FreezeAll;
                DoAttack = false;
                DoDash = false;
                _doBlock = false;
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
        interactableBlockTrigger.SetActive(false);
        switch (current)
        {
            case PlayerState.Dash:
                DashCooldown = dashDelay;
                _dashTime = dashDuration;
                _dashCollided.Clear();
                break;
            case PlayerState.Attack:
                AttackCooldown = attackDelay;
                break;
            case PlayerState.Blocking:
                playerAnimator.SetBool(IsAnimatingBlock, false);
                _blockAnimator.SetBool(IsAnimatingBlock, false);
                
                if (_blockCharge <= maxBlockCharge * 0.5f)
                {
                    _canBlock = false;
                }
                break;
            case PlayerState.InputRestricted:
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                                 RigidbodyConstraints.FreezeRotationZ;
                break;
        }

        if (newState != PlayerState.Movement && newState != PlayerState.Dash)
        { 
            Debug.Log("Exiting state: " + current + " to " + newState);
            playerAnimator.SetBool(IsAnimatingWalk, false);
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
                }
                
                DampenMovement();
                
                IdleToState();
                break;
            case PlayerState.Movement:
                if (MoveInput.magnitude <= 0f)
                {
                    SetPlayerState(PlayerState.Idle);
                }
                
                DampenMovement();

                IdleToState();
                break;
            case PlayerState.Dash:
                CheckIsDashing();
                break;
            case PlayerState.Blocking:
                if (!_doBlock || _blockCharge <= 0f)
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
        if (_doBlock)
        {
            SetPlayerState(PlayerState.Blocking);
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

        if (playerState == PlayerState.Blocking)
        {
            _blockCharge -= Time.deltaTime;
        }
        else if(_blockCharge < maxBlockCharge)
        {
            _blockCharge += Time.deltaTime;
            
            if (_blockCharge > maxBlockCharge)
            {
                _canBlock = true;
                _blockCharge = maxBlockCharge;
            }
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
            _isHoldingBlock = true;
        }

        if (context.canceled)
        {
            _isHoldingBlock = false;
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
        Physics.OverlapSphereNonAlloc(transform.position + LastMoveDirection * (dashHitRange / 2f), dashHitRange, _hitCollidersBuffer, layerMask);
        foreach (Collider hitCollider in _hitCollidersBuffer)
        {
            if (!hitCollider || hitCollider == col || _dashCollided.Contains(hitCollider) || hitCollider.CompareTag("BlockTrigger"))
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
            
            Array.Clear(_hitCollidersBuffer, 0, hitCollidersBufferSize);
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
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, blockDecelerationMultiplier * Time.fixedDeltaTime);
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

        Physics.OverlapSphereNonAlloc(transform.position + LastMoveDirection * attackRange, attackRadius, _hitCollidersBuffer, layerMask);
        foreach (Collider hitCollider in _hitCollidersBuffer)
        {
            if (!hitCollider || hitCollider == col || hitCollider.CompareTag("BlockTrigger"))
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
        
        Array.Clear(_hitCollidersBuffer, 0, hitCollidersBufferSize);
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

    public void AttackStun(CollisionData data)
    {
        if (playerState == PlayerState.Dash) return;

        if (playerState == PlayerState.Blocking)
        {
            _blockCharge = 0;
            _canBlock = false;
            onPlaySoundClipOnce.Raise(this, shieldBreakAudioClip);

            return;
        }
        
        Stun(data, stunTime);
    }

    public void OnInteractableBlock()
    {
        _blockCharge = 0;
        _canBlock = false;
        onPlaySoundClipOnce.Raise(this, shieldBreakAudioClip);
    }

    public void BracedReturnStun(CollisionData data)
    {
        Stun(data, blockReturnStunDuration);
    }

    private void Stun(CollisionData data, float duration)
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
        if (playerState != PlayerState.Blocking)
        {
            Knockback(data);
            player.onDashHasHit.Invoke();
        }
        else if (data.OriginCollider != null)
        {
            player.onDashBlockHit.Invoke(
                new CollisionData(-data.Direction, blockReturnKnockbackSpeed, col)
            );
        }
    }

    private void Knockback(CollisionData data)
    {
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(data.Direction * data.Speed, ForceMode.Impulse);
    }
}
