using System.Collections;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    public ProjectileConfig config { get; private set; }
    public Vector2 velocity;

    [SerializeField]
    protected CustomTrigger2D trigger;

    protected Rigidbody2D rb;
    private Collider2D ownCollider;
    private SpriteRenderer sr;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ownCollider = GetComponentInChildren<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        trigger.onEnter.AddListener(triggerEntered);
    }

    protected virtual void FixedUpdate()
    {
        transform.right = velocity.normalized;
        rb.linearVelocity = velocity;
    }

    protected virtual void triggerEntered(Collider2D col)
    {
        if (!col.enabled) return;

        // Compute a best-effort hit normal for triggers.
        Vector2 hitNormal = -velocity.normalized; // fallback
        //if (ownCollider != null && col != null)
        //{
        //    // Collider2D.Distance gives the minimal translation vector between colliders.
        //    // Its normal points from 'ownCollider' to 'col'.
        //    ColliderDistance2D d = ownCollider.Distance(col);
        //    Vector2 candidate = -d.normal; // outward normal of the hit surface (col)
        //    if (candidate.sqrMagnitude > 1e-6f)
        //    {
        //        hitNormal = candidate.normalized;
        //    }
        //    else
        //    {
        //        // Fallback: use closest point approximation
        //        Vector2 p = col.ClosestPoint(transform.position);
        //        Vector2 v = (Vector2)transform.position - p; // from surface to projectile
        //        if (v.sqrMagnitude > 1e-6f)
        //            hitNormal = v.normalized;
        //    }
        //}

        var damageable = col.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            if (!damageable.IsDead)
            {
                damageable.TakeDamage(
                    config.damage,
                    "hurt",
                    col.ClosestPoint(transform.position),
                    -velocity.normalized,
                    hitNormal,
                    config
                );
                GameEvents.Instance.OnFX?.Invoke(new FXEventData(transform.position, FXContext.hit, config, parent: col.transform));
                Destroy(gameObject);
            }
        }
        else
        {
            GameEvents.Instance.OnFX?.Invoke(new FXEventData(transform.position, FXContext.wall,  config, direction: hitNormal, parent: col.transform));
            Destroy(gameObject);
        }
    }

    public virtual void Initialize(Vector2 velocity, Transform caster, ProjectileConfig config)
    {
        this.config = config;
        this.velocity = velocity;
        if(sr!= null)
            sr.color = config.color;

        setLayerMask(config.layerMask.value);
        gameObject.AddComponent<LimitedLifetime>().Initialize(config.maxLifetime);
        transform.right = velocity.normalized;
        GameEvents.Instance.OnFX?.Invoke(new FXEventData(transform.position, FXContext.spawn, config, direction: velocity.normalized, parent: caster));
    }

    public virtual void setLayerMask(LayerMask hitLayers)
    {
        int layer = Mathf.RoundToInt(Mathf.Log(hitLayers.value, 2));
        SetLayerRecursively(transform, layer);
    }

    private void SetLayerRecursively(Transform obj, int layer)
    {
        obj.gameObject.layer = layer;
        foreach (Transform child in obj)
        {
            SetLayerRecursively(child, layer);
        }
    }
}