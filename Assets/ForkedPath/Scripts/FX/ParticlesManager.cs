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
        var entry = FXResolver.GetSerializedVFX(data);
        if (entry == null)
            return;

        var chosen = entry.prefabs.Length == 1
            ? entry.prefabs[0]
            : entry.prefabs[Random.Range(0, entry.prefabs.Length)];

        if (chosen == null) return;

        InstantiateWithContextRules(chosen, data);
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