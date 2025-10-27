using UnityEngine;
using Sirenix;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;
using System;

[System.Serializable]
public sealed class VFXParamsConfig : SerializedDictionary<string, SerializedVFX>
{
}

[System.Serializable]
public class SerializedVFX
{
    public GameObject[] prefabs = new GameObject[0];
}