using UnityEngine;

public class DrumstickController : EntityComponent, IFacingDirectionProvider, IMovementProvider, IShooterProvider
{
    [SerializeField]
    ProjectilesPattern shootingPattern;
    [SerializeField]
    Vector2 mouthPosition;
    Rigidbody2D rb;

    static readonly float WALK_TOWARDS_TIME = 0.5f;
    static readonly float PLAYER_TOO_CLOSE_DISTANCE = 4f;
    static readonly float WALK_SIDEWAYS_TIME = 1.5f;
    static readonly float WALK_SIDEWAYS_COOLDOWN = 3f;
    static readonly float SHOOTING_COOLDOWN = 3f;
    static readonly float AI_TICK = 0.2f;

    enum AIState
    {
        WalingTowards,
        Shooting,
        WalkingAway,
        WalkingSideways,
        Idle
    }
    EntitySpawnData spawnData;
    AIState currentState;

    AutomaticShooter automaticShooter;

    Vector2 lastNonZeroMoveDir = Vector2.down;
    bool isMoving;

    bool wasShooting;

    SimpleTimer aiTimer = new SimpleTimer();
    SimpleTimer walkTowardsTimer;
    SimpleTimer shootingCooldownTimer;
    SimpleTimer walkSidewaysTimer;
    SimpleTimer walkSidewaysCooldownTimer;

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

    Vector2 shootPosition => body.TransformPoint(mouthPosition);


    private void Start()
    {
        spawnData = GetComponent<EntitySpawnData>();
        
        automaticShooter = AutomaticShooter.ReloadAutomaticShooter(gameObject, shootingPattern);
        automaticShooter.OnShoot += OnShoot;

        //determine if we first walk towards
        if (spawnData != null)
        {

        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (automaticShooter != null)
        {
            automaticShooter.OnShoot -= OnShoot;
        }
    }


    private void Update()
    {
        
    }

    //AI check to change states
    void ProcessAITick()
    {
        aiTimer.reset(AI_TICK);

        if(currentState == AIState.WalingTowards)
        {
            if (walkTowardsTimer != null && !walkTowardsTimer.isTimeout())
                return;
        }

        if(shootingCooldownTimer == null || shootingCooldownTimer.isTimeout())
        {
            currentState = AIState.Shooting;
            shootingCooldownTimer = new SimpleTimer(SHOOTING_COOLDOWN);
            return;
        }

        if(walkSidewaysCooldownTimer == null || shootingCooldownTimer.isTimeout())
        {
            currentState = AIState.WalkingSideways;
            walkSidewaysCooldownTimer = new SimpleTimer(WALK_SIDEWAYS_COOLDOWN);
            return;
        }

        var 

        switch (currentState)
        {
            case AIState.WalingTowards:
                break;
            case AIState.Shooting:
                break;
            case AIState.WalkingAway:
                break;
            case AIState.WalkingSideways:
                break;
            case AIState.Idle:
                break;
            default:
                break;
        }
    }

    void Shoot()
    {
        automaticShooter.Shoot(Direction,);
        shootingCooldownTimer = new SimpleTimer(SHOOTING_COOLDOWN);
    }


    void OnShoot()
    {
        wasShooting = true;
    }
}