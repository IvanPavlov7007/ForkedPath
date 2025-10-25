using System.Collections;
using UnityEngine;

public class FXEventData
{
    public Vector2 position;
    public Vector2 direction;
    public Quaternion rotation;
    public string context; // e.g. "Impact", "EnemyDeath", "Spawn"
    public Transform parent; // optional parent transform

    public BaseConfig sourceConfig;

    public FXEventData(
        Vector2 pos,
        string context,
        BaseConfig sourceConfig,
        Transform parent = null,
        Vector2 direction = default)
    {
        position = pos;
        rotation = Quaternion.identity;
        this.context = context;
        this.parent = parent;
        this.direction = direction;
        this.sourceConfig = sourceConfig;
    }
}