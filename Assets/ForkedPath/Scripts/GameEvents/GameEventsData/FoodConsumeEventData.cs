using System.Collections;
using UnityEngine;

public class FoodConsumeEventData
{
    public Entity entity;
    public EntityFoodType foodType;
    public int count;
    public Vector2 position;

    public FoodConsumeEventData(Entity entity, EntityFoodType foodType, int count, Vector2 position)
    {
        this.entity = entity;
        this.foodType = foodType;
        this.count = count;
        this.position = position;
    }
}