using System;
using System.Collections;
using UnityEngine;

public partial class Entity : MonoBehaviour
{
    [SerializeField]
    private EntityConfig _config;
    public EntityConfig Config { get { return _config; } }

    public Health Health { get; protected set; }
    public Rigidbody2D Rb { get; protected set; }

    public EntityFoodType foodType;

    public bool IsInitialized { get; protected set; } = false;

    protected EntityState state = EntityState.Alive;
    public EntityState CurrentState => state;

    public event Action<EntityState, EntityStateChangeData> StateChanged;

    public bool isEatable
    {
        get
        {
            return foodType != EntityFoodType.NotEdible && (CurrentState == EntityState.Dead || CurrentState == EntityState.DeadFalling);
        }
    }

    protected Coroutine invincibilityCoroutine;
    protected Coroutine hitStunCoroutine; // NEW

    public virtual void Initialize(EntityConfig config)
    {
        if (IsInitialized) return;
        _config = config;
        foodType = config.initialFoodType;
        if (Health == null) Health = gameObject.AddComponent<Health>();
        if (Health != null && config != null)
        {
            Health.SetMaxHealth(config.maxHealth);
            Health.ResetHealth();

            // If corpseOnSpawn is true, set health and state to dead
            if (config.corpseOnSpawn)
            {
                Health.InitializeAsCorpse();
                state = EntityState.Dead; // Directly set state, do not call ChangeState to avoid events
            }
            else if (config.invincibleOnSpawn)
            {
                Health.BeginInvincibility(config.invincibilityDuration);
                state = EntityState.Invincible; // Directly set state, do not call ChangeState to avoid events
            }
        }
        IsInitialized = true;
    }

    public virtual void Eat(Entity eater)
    {
        if (!isEatable)
        {
            Debug.LogWarning($"{name} is not eatable in state {CurrentState} with food type {foodType}");
            return;
        }

        ChangeState(EntityState.Despawned, default);
        GameEvents.Instance.OnEntityEaten?.Invoke(new EatingEventData(eater,this));

    }

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if(!IsInitialized && Config != null)
            Initialize(Config);
    }

    protected virtual void OnEnable()
    {
        GameEvents.Instance.OnDeath += HandleDeath;
        GameEvents.Instance.OnDamage += HandleHit;
        GameEvents.Instance.OnInvincibilityChanged += OnInvincibilityChanged;
        GameEvents.Instance.OnFallingToDeathStarted += OnFallingToDeathStarted;
        GameEvents.Instance.OnCorpseLanded += OnCorpseLanded;
    }

    protected virtual void OnDisable()
    {
        GameEvents.Instance.OnDeath -= HandleDeath;
        GameEvents.Instance.OnDamage -= HandleHit;
        GameEvents.Instance.OnInvincibilityChanged -= OnInvincibilityChanged;
        GameEvents.Instance.OnFallingToDeathStarted -= OnFallingToDeathStarted;
        GameEvents.Instance.OnCorpseLanded -= OnCorpseLanded;

        CancelHitStun(); // NEW
    }

    protected virtual void ChangeState(EntityState newState, EntityStateChangeData changeData)
    {
        ExitState(state);
        if (state == newState) Debug.LogWarning($"{name} is reentering to same state {newState}");
        state = newState;
        EnterState(newState);
        StateChanged?.Invoke(state, changeData);
    }

    protected virtual void ExitState(EntityState oldState) { }
    protected virtual void EnterState(EntityState newState) {
        //if(newState == EntityState.Dead || newState == EntityState.DeadFalling)
        //{
        //    Rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        //}
    }

    protected virtual void HandleHit(DamageEventData damageEventData)
    {
        if (Health == null || damageEventData.target != Health as IDamageable) return;

        switch (CurrentState)
        {
            case EntityState.Alive:
            case EntityState.Hit:
                ChangeState(EntityState.Hit, new EntityStateChangeData() { damageEventData = damageEventData});

                bool willGoInvincible = Config.invincibleAfterHit && Health.CurrentHealth > 0;
                if (willGoInvincible)
                {
                    // invincibility owns returning to Alive via event
                    CancelHitStun();
                    Health.BeginInvincibility(Config.invincibilityDuration);
                }
                else
                {
                    float stun = Mathf.Max(0f, Config.hitStunDuration);
                    if (stun <= 0f)
                    {
                        ChangeState(EntityState.Alive, default);
                    }
                    else
                    {
                        RestartHitStun(stun);
                    }
                }
                break;

            case EntityState.Dead:
                Debug.LogError($"{name} is dead and should not take any damage");
                break;
            case EntityState.Falling:
                Debug.LogError($"{name} is falling and should not take any damage");
                break;
            case EntityState.Invincible:
                Debug.LogError($"{name} is invincible and should not take any damage");
                break;
        }
    }

    protected virtual void HandleDeath(DeathEventData deathEventData)
    {
        if(deathEventData.entity != this) return;

        // any death path should cancel pending hit-stun
        CancelHitStun();

        if(deathEventData.fallenToDeath)
        {
            switch(CurrentState)
            {
                case EntityState.Alive:
                case EntityState.Hit:
                case EntityState.Invincible:
                    Debug.LogError($"{name} in {CurrentState} is instantly dead from falling?");
                    //ChangeState(EntityState.Falling, default);
                    break;
                case EntityState.Falling:
                    // fall finalized: transition to dead now
                    ChangeState(EntityState.Dead, new EntityStateChangeData() { deathEventData = deathEventData });
                    break;
                case EntityState.Dead:
                    //ChangeState(EntityState.DeadFalling);
                    Debug.LogError($"{name} in {CurrentState} is instantly dead from falling?");
                    break;
                case EntityState.DeadFalling:
                    Debug.LogError($"{name} is already dead falling");
                    break;
            }
        }
        else
        {
            switch (CurrentState)
            {
                case EntityState.Alive:
                case EntityState.Hit:
                case EntityState.Invincible:
                    ChangeState(EntityState.Dead, new EntityStateChangeData() { deathEventData = deathEventData });
                    break;
                case EntityState.Dead:
                    Debug.LogError($"{name} is already dead");
                    break;
                case EntityState.Falling:
                    Debug.LogError($"{name} is falling and should not die again");
                    break;
            }
        }
    }

    void OnInvincibilityChanged(InvincibilityEventData e)
    {
        if (e.Entity != this) return;
        if (e.IsInvincible)
        {
            CancelHitStun(); // avoid racing the stun timer
            ChangeState(EntityState.Invincible, new EntityStateChangeData() { invincibilityEventData = e });
        }
        else if (CurrentState == EntityState.Invincible && !Health.IsDead)
        {
            ChangeState(EntityState.Alive, default);
        }
    }

    void OnFallingToDeathStarted(FallingEventData e)
    {
        if (e.entity != this) return;

        CancelHitStun(); // cancel stun if we start falling

        switch (CurrentState)
        {
            case EntityState.Alive:
            case EntityState.Hit:
            case EntityState.Invincible:
                ChangeState(EntityState.Falling, new EntityStateChangeData() { fallingEventData = e });
                break;
            case EntityState.Dead:
                ChangeState(EntityState.DeadFalling, new EntityStateChangeData() { fallingEventData = e });
                break;
            default:
                // if already Falling/DeadFalling, ignore
                Debug.LogWarning($"{name} should not start falling to death from state {CurrentState}");
                break;
        }
    }

    void OnCorpseLanded(CorpseLandedEventData e)
    {
        if (e.entity != this) return;
        if (CurrentState == EntityState.DeadFalling)
        {
            ChangeState(EntityState.Dead, new EntityStateChangeData() { corpseLandedEventData = e });
        }
    }

    // --- Hit-stun helpers ---
    void RestartHitStun(float duration)
    {
        CancelHitStun();
        hitStunCoroutine = StartCoroutine(HitStunDelay(duration));
    }

    void CancelHitStun()
    {
        if (hitStunCoroutine != null)
        {
            StopCoroutine(hitStunCoroutine);
            hitStunCoroutine = null;
        }
    }

    IEnumerator HitStunDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        hitStunCoroutine = null;

        // Only return to Alive if still stunned and not dead/falling
        if (CurrentState == EntityState.Hit && !Health.IsDead && !Health.IsFallingToDeath)
        {
            ChangeState(EntityState.Alive, default);
        }
    }
}