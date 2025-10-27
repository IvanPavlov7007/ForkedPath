using UnityEngine;

public class EntitySpawnedEventData
{
    public Entity entity;
    public EntityConfig config;
    public Vector2 position;

    public EntitySpawnedEventData(Entity entity, EntityConfig config, Vector2 position)
    {
        this.entity = entity;
        this.config = config;
        this.position = position;
    }
}