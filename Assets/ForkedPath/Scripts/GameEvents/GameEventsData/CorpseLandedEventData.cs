using UnityEngine;

public sealed class CorpseLandedEventData
{
    public Entity entity;

    public CorpseLandedEventData(Entity entity)
    {
        this.entity = entity;
    }
}