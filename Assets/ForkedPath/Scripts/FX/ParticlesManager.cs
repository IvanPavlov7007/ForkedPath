using System.Collections;
using UnityEngine;
using Pixelplacement;
public class ParticlesManager : Singleton<ParticlesManager>
{
    void OnEnable() => GameEvents.Instance.OnFX += HandleFX;
    void OnDisable() => GameEvents.Instance.OnFX -= HandleFX;

    void HandleFX(FXEventData data)
    {
        if (data.config == null) return;
        if (data.context == "Impact")
        {
            ProjectileConfig config = data.config as ProjectileConfig;
            if(config.impactFX)
                Destroy(Instantiate(config.impactFX, data.position, Quaternion.identity, data.parent).gameObject, 0.5f);
        }
        else if (data.context == "Spawn")
        {
            ProjectileConfig config = data.config as ProjectileConfig;
            if(config.spawnFX)
                Destroy(Instantiate(config.spawnFX, data.position, Quaternion.identity, data.parent).gameObject, 0.5f);
        }
        else if (data.context == "Hit")
        {
            EntityConfig config = data.config as EntityConfig;
            Debug.Log("Hit FX");
        }
        else if(data.context == "Explosions")
        {
            if(data.prefab != null)
                Instantiate(data.prefab, data.position, Quaternion.identity, data.parent);
        }
    }
}