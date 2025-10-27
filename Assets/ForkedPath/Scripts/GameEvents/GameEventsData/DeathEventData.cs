using UnityEngine;
public class DeathEventData
{
    public Entity entity;
    public DamageEventData damageEventData;
    public bool fallenToDeath;
    public DeathEventData(Entity entity, DamageEventData damageEventData = null, bool fallenToDeath = false)
    {
        this.entity = entity;
        this.damageEventData = damageEventData;
        this.fallenToDeath = fallenToDeath;
    }
}