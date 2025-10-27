using System.Collections;
using UnityEngine;
using Sirenix;
using Sirenix.OdinInspector;

public abstract class BaseConfig : ScriptableObject
{
    [Title("Common Data")]
    public string configId;
    [BoxGroup("Fallback Settings")]
    public bool useFallbackIfMissing = true;
    [BoxGroup("Fallback Settings")]
    [Tooltip("If enabled, this config will use the local fallback config, before a general fallback config when a referenced config is missing.")]
    public BaseConfig localFallbackConfig;
    [BoxGroup("Fallback Settings")]
    [ShowInInspector]
    public abstract BaseConfig GeneralFallbackConfig { get; }

    [Title("FX & Audio")]
    public AudioParamsConfig audioParams = new AudioParamsConfig();
    public VFXParamsConfig vfxParams = new VFXParamsConfig();
}