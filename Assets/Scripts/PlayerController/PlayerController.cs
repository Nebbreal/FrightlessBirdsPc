using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using EventSystem;
using MinigameFramework;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Initialization Settings")] 
    public PlayerState playerState;
    public Rigidbody rb;
    public Collider col;
    public Transform attackHitbox;
    public Transform dashHitbox;
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

    [Header("Walk Settings")] 
    public float acceleration = 20f;
    public float maxSpeed = 60f;
    public float maxSpeedClampFactor = 0.8f;
    public float defaultDecelerationMultiplier = 1.4f;
    [NonSerialized] 
    public Vector3 MoveInput = new Vector3(0f, 0f, 0f);
    private Vector3 _moveThisFrame = new Vector3(0f, 0f, 0f);
    public Animator playerAnimator;

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
    [Tooltip("In seconds")]
    public float maxBlockCharge = 3f;
    [NonSerialized]
    public float BlockCharge;
    public float blockReturnKnockbackSpeed = 2f;
    public float blockReturnStunDuration = 0.3f;
    public float blockDecelerationMultiplier = 0.9f;
    private bool _doBlock;
    private bool _isHoldingBlock;
    [NonSerialized]
    public bool CanBlock = true;
    public float requiredInteractableBreakVelocity = 8f;
    [SerializeField]
    private GameObject interactableBlockTrigger;
    private EggShield _eggShieldScript;
    private Animator _blockAnimator;

    [NonSerialized] 
    public Vector3 LastMoveDirection = new Vector3(1f, 0f, 0f);

    [Header("Game events")]
    [SerializeField] 
    private GameEvent onSpawnDebugPlayers;
    [SerializeField]
    public GameEvent onPlayerJoinLobby;
    [SerializeField]
    public GameEvent onPlayerLeaveLobby;
    public GameEvent onDashedInto;
    public GameEvent onAttacked;
    [SerializeField]
    private GameEvent onPlaySoundClipOnce;
    public GameEvent onSkipWinUI;

    private static readonly int IsAnimatingWalk = Animator.StringToHash("IsWalking");
    private static readonly int IsAnimatingBlock = Animator.StringToHash("IsBlocking");

    private Collider[] _hitCollidersBuffer;
    private float _currentMinigameDecelerationMultiplierOverride;    public GameEvent onPlayerDash;
    public GameEvent onPlayerAttack;
    public GameEvent onPlayerBrace;

    public void LoadFromSettings()
    {
        attackDelay = SettingsManager.Instance.settings.attackCooldown;
        dashDelay = SettingsManager.Instance.settings.dashCooldown;
        maxBlockCharge = SettingsManager.Instance.settings.blockCooldown;
        acceleration = SettingsManager.Instance.settings.playerMovementSpeed;
    }

    public void RestoreDefaultSettings()
    {
        SettingsManager.Instance.ReloadSettings();
    }

    private void Awake()
    {
        LoadFromSettings();
    }

    public void SetActive(bool state)
    {
        if (state)
        {
            playerAnimator = GetComponentInChildren<PlayerModel>().GetComponent<Animator>();
            _eggShieldScript = GetComponentInChildren<EggShield>();
            GameObject eggShieldObject = _eggShieldScript.gameObject;
            _blockAnimator = eggShieldObject.GetComponent<Animator>();
            _hitCollidersBuffer = new Collider[hitCollidersBufferSize];
            BlockCharge = maxBlockCharge;
        }
        
        IsActive = state;
    }

    private void Update()
    {
        if (!IsActive) return;
        
        if (playerState is PlayerState.InputRestricted)
        {
            DoAttack = false;
            _doBlock = false;
            DoDash = false;
            _moveThisFrame = Vector3.zero;
            return;
        }
        if (_isHoldingBlock && CanBlock && 
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
        FixedUpdateStates();

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
                onPlayerLeaveLobby.Raise(this, gameObject);
            }
            else
            {
                onPlayerJoinLobby.Raise(this, gameObject);
            }
        }
    }

    public void SpawnDebugPlayers(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        onSpawnDebugPlayers.Raise(this);
    }

    public void SkipWinUI(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (GameManager.Instance.IsWinUIActive && ID == 0) //Allow only host to skip win UI
        {  
            onSkipWinUI.Raise(this);
        }
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
                onPlayerDash.Raise(this);
                break;
            case PlayerState.Attack:
                _attackTime = attackDuration;
                _doBlock = false;
                DoDash = false;
                onPlayerAttack.Raise(this);
                break;
            case PlayerState.Stunned:
                _stunTimer = stunTime;
                stunTransform.gameObject.SetActive(true);
                DoAttack = false;
                DoDash = false;
                _doBlock = false;
                break;
            case PlayerState.Blocking:
                if (CanBlock && BlockCharge >= maxBlockCharge * 0.5)
                {
                    playerAnimator.SetBool(IsAnimatingBlock, true);
                    _blockAnimator.SetBool(IsAnimatingBlock, true);
                    
                    interactableBlockTrigger.SetActive(true);
                    DoAttack = false;
                    DoDash = false;
                    onPlayerBrace.Raise(this);
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
                break;
            case PlayerState.InputRestricted:
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                                 RigidbodyConstraints.FreezeRotationZ;
                break;
        }

        if (newState != PlayerState.Movement && newState != PlayerState.Dash)
        { 
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
                
                IdleToState();
                break;
            case PlayerState.Movement:
                if (MoveInput.magnitude <= 0f)
                {
                    SetPlayerState(PlayerState.Idle);
                }
                
                IdleToState();
                break;
            case PlayerState.Dash:
                CheckIsDashing();
                break;
            case PlayerState.Blocking:
                if (!_doBlock || BlockCharge <= 0f)
                {
                    SetPlayerState(PlayerState.Idle);
                }
                CheckIsBlocking();
                break;
            case PlayerState.Attack:
                CheckIsAttacking();
                break;
            case PlayerState.Stunned:
                CheckIsStunned();
                break;
        }
    }
    
    private void FixedUpdateStates()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                DampenMovement();
                break;
            case PlayerState.Movement:
                DampenMovement();
                break;
            case PlayerState.Blocking:
                CheckIsBlocking();
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
            BlockCharge -= Time.deltaTime;
        }
        else if(BlockCharge < maxBlockCharge)
        {
            BlockCharge += Time.deltaTime;
            
            if (BlockCharge > maxBlockCharge)
            {
                CanBlock = true;
                BlockCharge = maxBlockCharge;
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
        
        Vector3 move = new Vector3(moveValue.x, 0.0f, moveValue.y).normalized;
        
        MoveInput = move;
    }

    public void InputBlocking(InputAction.CallbackContext context)
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

    private void CheckIsBlocking()
    {
        _moveThisFrame = Vector3.zero;
        Vector3 newLinearVelocity =  Vector3.Lerp(rb.linearVelocity, Vector3.zero, blockDecelerationMultiplier * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(newLinearVelocity.x, rb.linearVelocity.y, newLinearVelocity.z);
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
        float decelerationMultiplier = _currentMinigameDecelerationMultiplierOverride != 0 ? 
            _currentMinigameDecelerationMultiplierOverride : 
            defaultDecelerationMultiplier;
        
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, decelerationMultiplier * Time.fixedDeltaTime);
    }

    public void UpdateDecelerationMultiplierOverride(Component sender, object[] data)
    {
        float decelMultOverride = 0;
        
        if (GameManager.Instance.IsMinigameSceneActive)
        {
            decelMultOverride = GameManager.Instance.CurrentMinigame.minigameSpecificDecelerationOverride;
        }
        
        _currentMinigameDecelerationMultiplierOverride = decelMultOverride;
    }
    
    public void ResetDecelerationMultiplierOverride()
    {
        _currentMinigameDecelerationMultiplierOverride = 0;
    }

    public void AttackStun(CollisionData data)
    {
        if (playerState == PlayerState.Dash || IsSameTeam(data)) return;

        if (playerState == PlayerState.Blocking)
        {
            BlockCharge = 0;
            CanBlock = false;
            _eggShieldScript.PlayBreakShieldSound();

            return;
        }
        
        Stun(data, stunTime);
        onAttacked.Raise(this, data);
    }

    public void OnInteractableBlock()
    {
        BlockCharge = 0;
        CanBlock = false;
        _eggShieldScript.PlayBreakShieldSound();
    }

    private bool IsSameTeam(CollisionData data)
    {
        var teams = GameManager.Instance.PlayerManager.Teams;
        if (teams == null)
        {
            return false;
        }

        GameObject originPlayer = data.OriginCollider.gameObject;
        foreach (var team in teams)
        {
            if (team.Players.Contains(gameObject) && team.Players.Contains(originPlayer))
            {
                return true;
            } 
        }

        return false;
    }

    public void BlockingReturnStun(CollisionData data)
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
            onDashedInto.Raise(this, data);
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
