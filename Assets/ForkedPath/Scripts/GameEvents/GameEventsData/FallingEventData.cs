using UnityEngine;

public sealed class FallingEventData
{
    public Entity entity;

    public FallingEventData(Entity entity)
    {
        this.entity = entity;
    }
}