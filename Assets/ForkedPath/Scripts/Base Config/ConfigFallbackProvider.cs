using System.Collections;
using UnityEngine;

public static class ConfigFallbackProvider
{
    public static T FallbackConfig<T>(T primaryConfig, T fallbackConfig) where T : ScriptableObject
    {
        return primaryConfig != null ? primaryConfig : fallbackConfig;
    }
}