using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class EntitySpawnDataContainer : MonoBehaviour
{
    public EntitySpawnData spawnData = new EntitySpawnData();
}

[System.Serializable]
public struct EntitySpawnData
{
    public Vector2 moveDirection;
    public SplineContainer splineContainer;
    public float magnitude;
}