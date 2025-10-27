using UnityEngine;
using Pixelplacement;
using UnityEngine.Rendering;
public class AudioManager : Singleton<AudioManager>
{
    //TODO: subscribe on scene load? Or should only AudioContoller be persistent?
    private void OnEnable() => GameEvents.Instance.OnFX += HandleFX;
    private void OnDisable() => GameEvents.Instance.OnFX -= HandleFX;

    void HandleFX(FXEventData data)
    {
        if(!TryGetSourceConfig(data, out var config) || config == null || config.audioParams == null)
        {
            return;
        }
        
        if(!TryGetSfxEntry(config, data.context, out var entry) || entry == null)
        {
            Debug.LogWarning($"AudioManager: No audio entry found for context '{data.context}' in config '{config.name}'");
            if (!TryGetSfxEntry(config.FallbackConfig, data.context, out var fallback) || fallback == null)
            {
                Debug.LogWarning($"AudioManager: No backup audio entry found for context '{data.context}' in config '{config.name}'");
                return;
            }
            entry = fallback;
        }

        PlayEntrySoundWithContextRules(entry, data);

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

    static bool TryGetSfxEntry(BaseConfig cfg, string context, out SerializedAudio entry)
    {
        entry = null;
        if (cfg?.audioParams == null || string.IsNullOrEmpty(context)) return false;
        if (cfg.audioParams.TryGetValue(context, out entry)) return true;
        // Case-insensitive fallback
        var ctxLower = context.ToLowerInvariant();
        foreach (var kvp in cfg.audioParams)
        {
            if (kvp.Key != null && kvp.Key.ToLowerInvariant() == ctxLower)
            {
                entry = kvp.Value;
                return true;
            }
        }
        return false;
    }

    static void PlayEntrySoundWithContextRules(SerializedAudio entry, FXEventData data)
    {
        if (entry == null) return;
        // Delegate selection and parameter application to AudioController
        AudioController.Instance.PlaySound3D(entry, data.position);
    }
}