using System.Collections;
using UnityEngine;

public class EatingEventData
{
    public Entity eater;
    public Entity prey;

    public EatingEventData(Entity eater, Entity prey)
    {
        this.eater = eater;
        this.prey = prey;
    }
}