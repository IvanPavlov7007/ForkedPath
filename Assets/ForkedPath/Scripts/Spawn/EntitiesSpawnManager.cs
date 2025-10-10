﻿using Pixelplacement;
using System;
using System.Collections;
using UnityEngine;

public class EntitiesSpawnManager : Singleton<EntitiesSpawnManager>
{
    public event Action<Entity> OnEntitySpawned;

    public Entity SpawnEntity(EntityConfig config, Vector2 position)
    {
        if (config == null || config.entityPrefab == null) throw new ArgumentNullException("Config or entityPrefab is null");

        GameObject entityInstance = Instantiate(config.entityPrefab, position, Quaternion.identity);
        Entity entity = entityInstance.GetComponent<Entity>();
        if (entity != null)
        {
            entity.Initialize(config);
            OnEntitySpawned?.Invoke(entity);
        }
        return entity;
    }
}