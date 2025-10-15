using UnityEngine;

public class DrumstickController : EntityComponent, IFacingDirectionProvider, IMovementProvider, IShooterProvider
{
    [SerializeField]
    ProjectilesPattern shootingPattern;
    [SerializeField]
    Vector2 mouthPosition;
    Rigidbody2D rb;
    EntitySpawnData spawnData;
    AutomaticShooter automaticShooter;

    Vector2 lastNonZeroMoveDir = Vector2.down;
    bool isMoving;

    bool wasShooting;

    SimpleTimer aiTimer = new SimpleTimer();

    // +1 = clockwise, -1 = counter-clockwise
    int sidewaysSign = 1;

    float MoveSpeed => entity.Config.moveSpeed;

    Entity CurrentPlayer => Player.Instance != null ? Player.Instance.CurrentAvatar : null;

    Vector2 DistanceToPlayer
    {
        get
        {
            var player = CurrentPlayer;
            if (player == null) return Vector2.positiveInfinity;
            return (Vector2)player.transform.position - (Vector2)transform.position;
        }
    }

    public Vector2 Direction
    {
        get
        {
            // Face movement if moving; otherwise face the player if available.
            var player = CurrentPlayer;
            if (isMoving) return lastNonZeroMoveDir;
            if (player != null)
            {
                Vector2 toPlayer = (Vector2)player.transform.position - (Vector2)transform.position;
                if (toPlayer.sqrMagnitude > 0.0001f) return toPlayer.normalized;
            }
            return lastNonZeroMoveDir;
        }
    }

    public bool IsMoving => isMoving;

    public Vector2 Velocity => rb.linearVelocity;


    public bool ConsumeShotThisFrame()
    {
        if (wasShooting)
        {
            wasShooting = false; 
            return true;
        }
        return false;
    }

    Vector2 shootPosition => body.TransformVector(mouthPosition);

    

    static readonly float PLAYER_TOO_CLOSE_DISTANCE = 3f;
    static readonly int SHOOT_COOLDOWN_CYCLES = 6;
    static readonly int WALK_TOWARDS_CYCTYLES = 2;
    static readonly float AI_TICK = 0.5f;

    bool walkTowardsIsAllowed = false;
    int cycles_to_shoot_left = 0;
    int walked_towards_cycles_count = 0;
    Vector2 lastPlayerPosition;

    enum AIState
    {
        WalkingTowards,
        Shooting,
        WalkingAway,
        WalkingSideways,
        Idle
    }
    
    AIState currentState;


    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        spawnData = GetComponent<EntitySpawnData>();
        
        automaticShooter = AutomaticShooter.ReloadAutomaticShooter(gameObject, shootingPattern);
        automaticShooter.OnShoot += OnShoot;

        //determine if we first walk towards
        if (spawnData != null)
        {
            walkTowardsIsAllowed = spawnData.moveDirection.sqrMagnitude > 0.01f;
        }

        aiTimer = new SimpleTimer(AI_TICK);
        cycles_to_shoot_left = SHOOT_COOLDOWN_CYCLES;
        ProcessAITick();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (automaticShooter != null)
        {
            automaticShooter.OnShoot -= OnShoot;
        }
    }

    protected override void OnDeath(DeathEventData deathEventData)
    {
        rb.linearVelocity = Vector2.zero;
        if (automaticShooter != null)
            automaticShooter.StopShooting();
        enabled = false;
    }


    private void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;

        if (aiTimer.tick(deltaTime))
        {
            ProcessAITick();
        }


        switch (currentState)
        {
            case AIState.WalkingTowards:
                rb.linearVelocity = spawnData.moveDirection * MoveSpeed;
                break;
            case AIState.Shooting:
                isMoving = false;
                Shoot();
                break;
            case AIState.WalkingAway:
                rb.linearVelocity = ((Vector2)transform.position - lastPlayerPosition).normalized * MoveSpeed * 0.2f;
                break;
            case AIState.WalkingSideways:
                Vector2 towardsPlayer = (lastPlayerPosition - (Vector2)transform.position).normalized;
                Vector2 sidewaysDir = new Vector2(-towardsPlayer.y, towardsPlayer.x).normalized * sidewaysSign;
                rb.linearVelocity = sidewaysDir * MoveSpeed * 0.5f;
                break;
            case AIState.Idle:
                break;
            default:
                break;
        }
        isMoving = rb.linearVelocity.sqrMagnitude > 0.01f;
        if(isMoving)
            lastNonZeroMoveDir = rb.linearVelocity.normalized;

    }

    //AI check to change states
    void ProcessAITick()
    {
        aiTimer.reset(AI_TICK);

        if (walkTowardsIsAllowed && walked_towards_cycles_count < WALK_TOWARDS_CYCTYLES)
        {
            currentState = AIState.WalkingTowards;
            walked_towards_cycles_count++;
            return;
        }

        if (--cycles_to_shoot_left == 0)
        {
            rb.linearVelocity = Vector2.zero;
            currentState = AIState.Shooting;
            cycles_to_shoot_left = SHOOT_COOLDOWN_CYCLES;
            if (automaticShooter != null)// to reset shooting, refactor later
                automaticShooter.StopShooting();
            return;
        }

        var player = CurrentPlayer;

        if (CurrentPlayer == null)
        {
            rb.linearVelocity = Vector2.zero;
            currentState = AIState.Idle;
            return;
        }

        lastPlayerPosition = player.transform.position;

        if (DistanceToPlayer.magnitude < PLAYER_TOO_CLOSE_DISTANCE)
        {
            currentState = AIState.WalkingAway;
            return;
        }

        sidewaysSign = Random.value > 0.5f ? 1 : -1;
        currentState = AIState.WalkingSideways;

    }

    void Shoot()
    {
        if (automaticShooter != null)
        {
            automaticShooter.Shoot(Direction, shootPosition);

        }
        else
            Debug.LogWarning($"{gameObject.name} has no AutomaticShooter component");
    }


    void OnShoot()
    {
        wasShooting = true;
    }
}