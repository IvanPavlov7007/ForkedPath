using System.Collections;
using UnityEngine;

public class EntityBaseAudio : EntityComponent
{
    protected override void OnDeath(DeathEventData deathEventData)
    {
        GameEvents.Instance.OnFX?.Invoke(new FXEventData(transform.position, FXContext.death, entity.Config, parent: transform));
    }

    protected override void OnHit(DamageEventData damageEventData)
    {
        GameEvents.Instance.OnFX?.Invoke(new FXEventData(transform.position, FXContext.hit, entity.Config, parent: transform));
    }

    protected override void OnFalling(FallingEventData e)
    {
        GameEvents.Instance.OnFX?.Invoke(new FXEventData(transform.position, FXContext.fall, entity.Config, parent: transform));
    }

    protected override void InstantDie() { }
}