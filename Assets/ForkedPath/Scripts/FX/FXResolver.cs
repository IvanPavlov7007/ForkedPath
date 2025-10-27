using System.Collections;
using UnityEngine;
public static class FXResolver
{
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
        if (cfg == null)
        {
            return false;
        }
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

    static bool TryGetSfxEntryRecursive(BaseConfig cfg, string context, out SerializedAudio entry)
    {
        entry = null;
        if (TryGetSfxEntry(cfg, context, out entry) && entry != null)
        {
            return true;
        }
        if (cfg.useFallbackIfMissing)
        {
            if (TryGetSfxEntryRecursive(cfg.localFallbackConfig, context, out entry) && entry != null)
            {
                return true;
            }
            if (TryGetSfxEntryRecursive(cfg.GeneralFallbackConfig, context, out entry) && entry != null)
            {
                return true;
            }
        }
        return false;
    }

    public static SerializedAudio GetSerializedAudio(FXEventData data)
    {
        if (!TryGetSourceConfig(data, out var config) || config == null || config.audioParams == null)
        {
            return null;
        }

        if (!TryGetSfxEntryRecursive(config, data.context, out var entry) || entry == null)
        {
            Debug.LogWarning($"AudioManager: No audio entry found for context '{data.context}' in config '{config.name}' and it's fallbacks");
        }

        return entry;
    }


    static bool TryGetVfxEntry(BaseConfig cfg, string context, out SerializedVFX entry)
    {
        entry = null;
        if (cfg == null)
        {
            return false;
        }
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

    static bool TryGetVfxEntryRecursive(BaseConfig cfg, string context, out SerializedVFX entry)
    {
        entry = null;
        if (TryGetVfxEntry(cfg, context, out entry) && entry != null)
        {
            return true;
        }
        if (cfg.useFallbackIfMissing)
        {
            if (TryGetVfxEntryRecursive(cfg.localFallbackConfig, context, out entry) && entry != null)
            {
                return true;
            }
            if (TryGetVfxEntryRecursive(cfg.GeneralFallbackConfig, context, out entry) && entry != null)
            {
                return true;
            }
        }
        return false;
    }

    public static SerializedVFX GetSerializedVFX(FXEventData data)
    {
        if (!TryGetSourceConfig(data, out var config) || config == null || config == null)
        {
            return null;
        }

        if (!TryGetVfxEntryRecursive(config, data.context, out var entry) || entry == null)
        {
            Debug.LogWarning($"AudioManager: No audio entry found for context '{data.context}' in config '{config.name}' ot it's fallbacks");
        }

        return entry;
    }

}