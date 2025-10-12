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


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trigger.onEnter.AddListener(triggerEntered);
    }

    protected virtual void FixedUpdate()
    {
        transform.right = velocity.normalized;
        rb.linearVelocity = velocity;
    }

    protected virtual void triggerEntered(Collider2D col)
    {
        var damageable = col.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            if (!damageable.IsDead)
            {
                damageable.TakeDamage(config.damage, "Projectile", col.ClosestPoint(transform.position), -velocity.normalized, config);
                GameEvents.Instance.OnFX?.Invoke(new FXEventData(transform.position, "Impact", config, parent: col.transform));
                Destroy(gameObject);
            }
        }
        else
        {
            GameEvents.Instance.OnFX?.Invoke(new FXEventData(transform.position, "Impact", config, parent: col.transform));
            Destroy(gameObject);
        }

    }

    public virtual void Initialize(Vector2 velocity, Transform caster, ProjectileConfig config)
    {
        this.config = config;
        this.velocity = velocity;
        setLayerMask(config.layerMask.value);
        gameObject.AddComponent<LimitedLifetime>().Initialize(config.maxLifetime);
        transform.right = velocity.normalized;
        GameEvents.Instance.OnFX?.Invoke(new FXEventData(transform.position, "Spawn", config, parent: caster));
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