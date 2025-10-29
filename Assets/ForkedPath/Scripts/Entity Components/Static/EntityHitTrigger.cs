using System;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class EntityHitTrigger : EntityComponent
{
    protected override void Awake()
    {
        base.Awake();
        foreach (var trigger in GetComponentsInChildren<CustomTrigger2D>())
        {
            trigger.onEnter.AddListener(OnTriggerEntered);
        }
    }

    protected virtual void OnTriggerEntered(Collider2D col)
    {
        if(enabled == false)
            return;
        var targetEntity = col.GetComponentInParent<Entity>();
        if (entity == null || targetEntity == null || targetEntity == entity)
            return;
        if (entity.Health.IsDead)
            return;

        // Only interact with alive entities on allowed layers
        if (targetEntity.CurrentState == EntityState.Alive)
        {
            if ((entity.Config.interactWithAliveLayers.value & (1 << col.gameObject.layer)) == 0)
                return;

            var damageable = col.GetComponentInParent<IDamageable>();
            if (damageable != null && !damageable.IsDead)
            {
                var hitPos = col.ClosestPoint(transform.position);
                Vector2 hitNormal = (hitPos - (Vector2)transform.position).normalized;
                damageable.TakeDamage(entity.Config.collisionDamage, "hurt", hitPos, col.transform.position - transform.position, hitNormal, entity.Config);
                GameEvents.Instance.OnFX?.Invoke(new FXEventData(hitPos, FXContext.hit, sourceConfig: entity.Config, parent: col.transform));
            }
            
        }
        else if (targetEntity.CurrentState == EntityState.Dead)
        {
            // Example: Player eats dead enemy
            // Implement your "eat" logic here
            // Possibly call a method on targetEntity to handle being eaten
        }
    }

    protected override void InstantDie()
    {
        enabled = false;
    }
}