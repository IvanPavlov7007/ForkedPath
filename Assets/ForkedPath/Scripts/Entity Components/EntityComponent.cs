using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using Pixelplacement.TweenSystem;

[RequireComponent(typeof(Entity))]
public abstract class EntityComponent : MonoBehaviour
{
    public Entity entity { get; protected set; }
    protected Transform body;
    protected SpriteRenderer spriteRenderer;

    protected readonly List<TweenBase> activeTweens = new List<TweenBase>();

    protected bool isDead => entity != null && entity.CurrentState == EntityState.Dead;

    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        body = transform.Find("body");
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        if(entity.CurrentState == EntityState.Dead)
        {
            InstantDie();
        }
    }

    protected abstract void InstantDie();

    protected virtual void OnEnable()
    {
        if (entity == null) entity = GetComponent<Entity>();
        if (entity != null)
        {
            entity.StateChanged += OnStateChanged;
        }
        ResetVisualState();
    }

    protected virtual void OnDisable()
    {
        if (entity != null)
        {
            entity.StateChanged -= OnStateChanged;
        }
        CancelTweens();
        if (spriteRenderer != null) spriteRenderer.enabled = true;
    }

    protected virtual void OnHit(DamageEventData damageEventData)
    {
    }

    protected void CancelTweens()
    {
        foreach (var tween in activeTweens)
        {
            tween?.Cancel();
        }
        activeTweens.Clear();
    }

    protected virtual void OnDeath(DeathEventData deathEventData)
    {
        // Hook for death VFX/animation.
    }

    protected virtual void OnInvincibility(InvincibilityEventData e)
    {
    }

    // Optional: override in subclasses to add falling visuals
    protected virtual void OnFalling(FallingEventData e)
    {
        // e.g., play a fall animation or trail/scale effect
    }

    protected virtual void OnStateChanged(EntityState newState, EntityStateChangeData data)
    {
        switch (newState)
        {
            case EntityState.Alive:
                break;
            case EntityState.Dead:
                OnDeath(data.deathEventData);
                break;
            case EntityState.Hit:
                OnHit(data.damageEventData);
                break;
            case EntityState.Falling:
                OnFalling(data.fallingEventData);
                break;
            case EntityState.Invincible:
                OnInvincibility(data.invincibilityEventData);
                break;
            case EntityState.DeadFalling:
                break;
            default:
                break;
        }
    }

    protected virtual void ResetVisualState()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = true;
    }
}