using UnityEngine;

public class DrumstickController : EntityComponent, IFacingDirectionProvider, IMovementProvider, IShooterProvider
{
    [SerializeField]
    ProjectilesPattern shootingPattern;

    static readonly float WALK_TOWARDS_DISTANCE = 5f;
    static readonly float PLAYER_TOO_CLOSE_DISTANCE = 4f;
    static readonly float WALK_SIDEWAYS_TIME = 1.5f;
    static readonly float WALK_SIDEWAYS_COOLDOWN = 3f;
    static readonly float SHOOTING_COOlDOWN = 3f;
    static readonly float AI_TICK = 0.2f;

    enum AIState
    {
        WalingTowards,
        WalkingAway,
        WalkingSideways,
        Shooting
    }
    EntitySpawnData spawnData;
    AIState currentState;

    public Vector2 Direction => throw new System.NotImplementedException();

    public bool IsMoving => throw new System.NotImplementedException();

    public Vector2 Velocity => throw new System.NotImplementedException();

    public bool ConsumeShotThisFrame()
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        spawnData = GetComponent<EntitySpawnData>();
        
        //determine if we first walk towards
        if (spawnData != null)
        {

        }
    }

    private void Update()
    {
        
    }

    void OnShoot()
    {

    }
}