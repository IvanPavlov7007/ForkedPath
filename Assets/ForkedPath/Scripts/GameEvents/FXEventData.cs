using System.Collections;
using UnityEngine;

public class FXEventData
{
    public Vector2 position;
    public Vector2 hitNormal;
    public Quaternion rotation;
    public string context; // e.g. "Impact", "EnemyDeath", "Spawn"
    public GameObject prefab; // optional direct prefab override
    public AudioClip sound;   // optional SFX
    public Color color;       // optional color tint
    public float intensity;   // for shake or scaling
    public Transform parent; // optional parent transform

    public ProjectileConfig projectile_config;
    public DamageEventData damage_event_data;
    public EntityConfig entity_config;

    public FXEventData(Vector2 pos, string ctx, ProjectileConfig projectileConfig = null, DamageEventData damageEventData = null, EntityConfig entityConfig = null, GameObject fx = null, AudioClip sfx = null, Transform parent = null, Vector2 hitNormal = default)
    {
        position = pos;
        rotation = Quaternion.identity;
        context = ctx;
        prefab = fx;
        sound = sfx;
        color = Color.white;
        intensity = 1f;
        this.parent = parent;
        projectile_config = projectileConfig;
        damage_event_data = damageEventData;
        this.entity_config = entityConfig;
        this.hitNormal = hitNormal;
    }
}