using System.Collections;
using UnityEngine;
using Sirenix;
using Sirenix.OdinInspector;

public abstract class BaseConfig : ScriptableObject
{
    [Title("Common Data")]
    public string configId;
    public bool useFallbackIfMissing = true;

    [Title("FX & Audio")]
    public AudioParamsConfig audioParams;
    public VFXParamsConfig vfxParams;

    public abstract BaseConfig FallbackConfig { get; }
}