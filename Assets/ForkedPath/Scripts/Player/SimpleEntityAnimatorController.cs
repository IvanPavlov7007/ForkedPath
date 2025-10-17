using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class SimpleEntityAnimatorController : EntityComponent
{
    protected IMovementProvider movementProvider;
    protected IFacingDirectionProvider facingDirectionProvider;
    protected IShooterProvider shooterProvider;
    protected Animator anim;
    protected static readonly int DeadHash = Animator.StringToHash("Dead");
    protected static readonly int ShootHash = Animator.StringToHash("Shoot");
    protected static readonly int WalkingHash = Animator.StringToHash("Walking");
    protected static readonly int XHash = Animator.StringToHash("X");
    protected static readonly int YHash = Animator.StringToHash("Y");



    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        movementProvider = GetComponent<IMovementProvider>();
        facingDirectionProvider = GetComponent<IFacingDirectionProvider>(); 
        shooterProvider = GetComponent<IShooterProvider>();
    }

    protected virtual void Update()
    {
        if (facingDirectionProvider != null)
        {
            anim.SetFloat(XHash, facingDirectionProvider.Direction.x);
            anim.SetFloat(YHash, facingDirectionProvider.Direction.y);
        }
        if(movementProvider != null)
            anim.SetBool(WalkingHash, movementProvider.IsMoving);
        if(shooterProvider != null && shooterProvider.ConsumeShotThisFrame())
            anim.SetTrigger(ShootHash);
    }

    protected override void OnDeath(DeathEventData deathEventData)
    {
        InstantDie();
    }

    protected override void InstantDie()
    {
        anim.SetBool(DeadHash, true);
    }
}

// Interfaces moved to stand‑alone definitions:

public interface IMovementProvider
{
    bool IsMoving { get; }
    Vector2 Velocity { get; }
}

public interface IFacingDirectionProvider
{
    // Should be normalized or zero if idle.
    Vector2 Direction { get; }
}

public interface IShooterProvider
{
    // Returns true only once per fired shot.
    bool ConsumeShotThisFrame();
}