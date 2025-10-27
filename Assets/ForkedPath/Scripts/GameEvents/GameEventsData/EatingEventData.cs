using System.Collections;
using UnityEngine;

public class EatingEventData
{
    public Entity eater;
    public Entity prey;
    public Vector2 position;

    public EatingEventData(Entity eater, Entity prey)
    {
        this.eater = eater;
        this.prey = prey;
    }
}