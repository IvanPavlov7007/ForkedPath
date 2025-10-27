using UnityEngine;
using Sirenix;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;
using System;

[Serializable]
public sealed class VFXParamsConfig : SerializedDictionary<string, SerializedVFX>
{
}

public class SerializedVFX
{
    public GameObject[] prefabs;
}