using System.Collections;
using UnityEngine;
using Pixelplacement;
public class ParticlesManager : Singleton<ParticlesManager>
{
    const float DefaultFxLifetime = 0.5f;

    void OnEnable() => GameEvents.Instance.OnFX += HandleFX;
    void OnDisable() => GameEvents.Instance.OnFX -= HandleFX;

    void HandleFX(FXEventData data)
    {
        if (!TryGetSourceConfig(data, out var cfg) || cfg == null || cfg.vfxParams == null)
            return;

        // 3) Lookup by context in dictionary
        if (!TryGetVfxEntry(cfg, data.context, out var entry) || entry?.prefabs == null || entry.prefabs.Length == 0)
            return;

        var chosen = entry.prefabs.Length == 1
            ? entry.prefabs[0]
            : entry.prefabs[Random.Range(0, entry.prefabs.Length)];

        if (chosen == null) return;

        InstantiateWithContextRules(chosen, data);
    }

    static bool TryGetSourceConfig(FXEventData data, out BaseConfig cfg)
    {
        if (data.sourceConfig != null)
        {
            cfg = data.sourceConfig;
            return true;
        }

        cfg = null;
        return false;
    }

    static bool TryGetVfxEntry(BaseConfig cfg, string context, out SerializedVFX entry)
    {
        entry = null;
        if (cfg?.vfxParams == null || string.IsNullOrEmpty(context)) return false;

        if (cfg.vfxParams.TryGetValue(context, out entry)) return true;

        // Case-insensitive fallback
        var ctxLower = context.ToLowerInvariant();
        foreach (var kvp in cfg.vfxParams)
        {
            if (kvp.Key != null && kvp.Key.ToLowerInvariant() == ctxLower)
            {
                entry = kvp.Value;
                return true;
            }
        }
        return false;
    }

    static void InstantiateWithContextRules(GameObject prefab, FXEventData data)
    {
        var contextLower = (data.context ?? string.Empty).ToLowerInvariant();

        Quaternion rotation = Quaternion.identity;
        if ((contextLower == "wall" || contextLower == "spawn") && data.direction != Vector2.zero)
        {
            rotation = Quaternion.FromToRotation(Vector2.up, data.direction);
        }

        Transform parent = (contextLower == "hit") ? null : data.parent;

        var go = Object.Instantiate(prefab, data.position, rotation, parent);

        if (contextLower == "explosion")
        {
            return;
        }

        Object.Destroy(go, DefaultFxLifetime);
    }
}