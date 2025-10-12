using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using Pixelplacement.TweenSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Entity))]
public abstract class EntityVisualsBase : MonoBehaviour
{
    protected Entity entity;
    protected Transform body;

    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    private readonly List<TweenBase> activeTweens = new List<TweenBase>();

    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        body = transform.Find("body");
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

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
        if (body == null || spriteRenderer == null) return;
        activeTweens.Add(Tween.Shake(body, body.localPosition, new Vector2(1f, 0.2f), 0.1f, 0f));
        activeTweens.Add(Tween.Color(spriteRenderer, Color.red, 0.05f, 0f));
        activeTweens.Add(Tween.Color(spriteRenderer, Color.white, 0.05f, 0.05f));
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
        if (spriteRenderer == null) return;

        float blinkInterval = 0.1f;
        int blinkCount = Mathf.CeilToInt(e.Duration / blinkInterval);

        IEnumerator BlinkCoroutine()
        {
            for (int i = 0; i < blinkCount; i++)
            {
                spriteRenderer.enabled = false;
                yield return new WaitForSeconds(blinkInterval / 2f);
                spriteRenderer.enabled = true;
                yield return new WaitForSeconds(blinkInterval / 2f);
            }
            spriteRenderer.enabled = true;
        }

        StartCoroutine(BlinkCoroutine());
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