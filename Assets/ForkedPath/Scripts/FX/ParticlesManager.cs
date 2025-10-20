using System.Collections;
using UnityEngine;
using Pixelplacement;
public class ParticlesManager : Singleton<ParticlesManager>
{
    void OnEnable() => GameEvents.Instance.OnFX += HandleFX;
    void OnDisable() => GameEvents.Instance.OnFX -= HandleFX;

    void HandleFX(FXEventData data)
    {
        if (data.context == "Impact")
        {
            ProjectileConfig config = data.projectile_config;
            if (config != null &&  config.impactFX)
                Destroy(Instantiate(config.impactFX, data.position, Quaternion.identity, null).gameObject, 0.5f);
        }
        else if(data.context == "Wall")
        {
            ProjectileConfig config = data.projectile_config;
            if (config != null &&  config.wallFX)
                Destroy(Instantiate(config.wallFX, data.position, Quaternion.FromToRotation(Vector2.up, data.hitNormal), data.parent).gameObject, 0.5f);
        }
        else if (data.context == "Spawn")
        {
            ProjectileConfig config = data.projectile_config;
            if (config != null && config.spawnFX)
                Destroy(Instantiate(config.spawnFX, data.position, Quaternion.FromToRotation(Vector2.up, data.hitNormal), data.parent).gameObject, 0.5f);
        }
        else if (data.context == "Hit")
        {
            EntityConfig config = data.entity_config;
            Debug.Log("Hit FX");
        }
        else if (data.context == "Explosions")
        {
            if (data.prefab != null)
                Instantiate(data.prefab, data.position, Quaternion.identity, data.parent);
        }
    }
}