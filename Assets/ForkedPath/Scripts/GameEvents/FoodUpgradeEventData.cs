using System.Collections;
using UnityEngine;
public class FoodUpgradeEventData 
{
    public Entity entity;
    public FoodRulesConfig foodRulesConfig;
    public EntityFoodType currentFoodType;
    public int newLevel;

    public FoodUpgradeEventData(Entity entity, FoodRulesConfig foodRulesConfig, EntityFoodType currentFoodType, int newLevel)
    {
        this.entity = entity;
        this.foodRulesConfig = foodRulesConfig;
        this.currentFoodType = currentFoodType;
        this.newLevel = newLevel;
    }
}