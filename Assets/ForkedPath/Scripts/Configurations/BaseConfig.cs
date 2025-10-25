using System.Collections;
using UnityEngine;
using Sirenix;
using Sirenix.OdinInspector;

public class BaseConfig : ScriptableObject
{
    [Title("Common Data")]
    public string configId;

    [Title("FX & Audio")]
    public AudioParamsConfig audioParams;
    public VFXParamsConfig vfxParams;
}